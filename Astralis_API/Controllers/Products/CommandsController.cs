using Astralis.Shared.DTOs;
using Astralis_API.Configuration;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout; // --- AJOUT POUR STRIPE ---
using System.ComponentModel;
using System.Security.Claims;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("Command")]
    public class CommandsController : CrudController<Command, CommandListDto, CommandDetailDto, CommandCheckoutDto, CommandUpdateDto, int>
    {
        private readonly ICommandRepository _commandRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        // --- AJOUT : Configuration pour lire la clé Stripe ---
        private readonly IConfiguration _configuration;

        // --- MODIFICATION DU CONSTRUCTEUR ---
        public CommandsController(ICommandRepository commandRepository,
            ICartItemRepository cartItemRepository,
            IOrderDetailRepository orderDetailRepository,
            IMapper mapper,
            IConfiguration configuration) // On injecte la configuration ici
            : base(commandRepository, mapper)
        {
            _commandRepository = commandRepository;
            _cartItemRepository = cartItemRepository;
            _orderDetailRepository = orderDetailRepository;
            _configuration = configuration; // On stocke la configuration
        }

        /// <summary>
        /// Retrieves all commands.
        /// </summary>
        /// <remarks>Admins see all commands. Users see only their own history.</remarks>
        /// <returns>A list of commands.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<IEnumerable<CommandListDto>>> GetAll()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            IEnumerable<Command> allCommands = await _repository.GetAllAsync();

            if (userRole != "Admin")
            {
                allCommands = allCommands.Where(c => c.UserId == userId);
            }

            allCommands = allCommands.OrderByDescending(c => c.Date);

            return Ok(_mapper.Map<IEnumerable<CommandListDto>>(allCommands));
        }

        /// <summary>
        /// Retrieves a specific command by ID.
        /// </summary>
        /// <param name="id">Command ID.</param>
        /// <returns>The command details including items.</returns>
        /// <response code="200">Command found.</response>
        /// <response code="403">Forbidden (not your command).</response>
        /// <response code="404">Command not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult<CommandDetailDto>> GetById(int id)
        {
            Command? entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            if (userRole != "Admin" && entity.UserId != userId)
            {
                return Forbid();
            }

            return Ok(_mapper.Map<CommandDetailDto>(entity));
        }

        // --- AJOUT : Méthode de validation post-paiement Stripe ---
        [HttpPost("validate-payment")]
        public async Task<IActionResult> ValidatePayment([FromQuery] string sessionId)
        {
            // 1. Récupération User
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            try
            {
                // 2. Vérification Stripe
                Stripe.StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                if (session.PaymentStatus != "paid")
                {
                    return BadRequest("Le paiement n'a pas été validé par la banque.");
                }

                // 3. Récupération du panier
                IEnumerable<CartItem> cartItems = await _cartItemRepository.GetByUserIdAsync(userId);
                if (!cartItems.Any())
                {
                    // Si le panier est déjà vide, la commande a probablement déjà été traitée
                    return Ok(new { message = "Commande déjà traitée." });
                }

                // 4. Création de la Commande (Status 2 = Payée)
                Command command = new Command
                {
                    UserId = userId,
                    CommandStatusId = 2, // 2 = Payé/Validé
                    Date = DateTime.UtcNow,
                    Total = (decimal)(session.AmountTotal ?? 0) / 100m, // On prend le montant réel payé chez Stripe
                    PdfName = $"Facture_{DateTime.Now:yyyyMMdd}_{userId}.pdf",
                    PdfPath = "/content/invoices/placeholder.pdf"
                };

                await _repository.AddAsync(command);

                // 5. Création des lignes (OrderDetails)
                List<OrderDetail> orderDetails = cartItems.Select(item => new OrderDetail
                {
                    CommandId = command.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList();

                await _orderDetailRepository.AddRangeAsync(orderDetails);

                // 6. Vider le panier
                await _cartItemRepository.ClearCartAsync(userId);

                return Ok(new { commandId = command.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Validates a checkout and creates a new command (Manual Checkout).
        /// </summary>
        /// <param name="checkoutDto">Checkout details (Addresses, Payment).</param>
        /// <returns>The created command.</returns>
        /// <response code="200">Command successfully created.</response>
        /// <response code="400">Invalid checkout data (empty cart, invalid payment).</response>
        /// <response code="401">User not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<CommandDetailDto>> Post(CommandCheckoutDto checkoutDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            IEnumerable<CartItem> cartItems = await _cartItemRepository.GetByUserIdAsync(userId);

            if (!cartItems.Any())
            {
                return BadRequest("Cannot checkout an empty cart.");
            }

            decimal totalAmount = cartItems.Sum(item => item.Quantity * item.ProductNavigation.Price);

            Command command = new Command
            {
                UserId = userId,
                CommandStatusId = 1,
                Date = DateTime.UtcNow,
                Total = totalAmount,
                PdfName = $"Facture_{DateTime.Now:yyyyMMdd}_{userId}.pdf",
                PdfPath = "/content/invoices/placeholder.pdf"
            };

            await _repository.AddAsync(command);

            List<OrderDetail> orderDetails = cartItems.Select(item => new OrderDetail
            {
                CommandId = command.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            await _orderDetailRepository.AddRangeAsync(orderDetails);

            await _cartItemRepository.ClearCartAsync(userId);

            return Ok(_mapper.Map<CommandDetailDto>(command));
        }

        /// <summary>
        /// Updates a command status (Admin only).
        /// </summary>
        /// <param name="id">Command ID.</param>
        /// <param name="updateDto">New status.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The command was successfully updated.</response>
        /// <response code="403">Forbidden (not your command).</response>
        /// <response code="404">The command does not exist.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> Put(int id, CommandUpdateDto updateDto)
        {
            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a command (Admin only).
        /// </summary>
        /// <remarks>Should strictly be used for cancelling erroneous orders.</remarks>
        /// <response code="204">The command was successfully deleted.</response>
        /// <response code="403">Forbidden (not your command).</response>
        /// <response code="404">The command does not exist.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}