using Astralis.Shared.DTOs;
using Astralis_API.Configuration;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Services.Interfaces; // Pour IEmailService
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.ComponentModel;
using System.Net;
using System.Security.Claims;
using System.Text; // Pour StringBuilder

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

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public CommandsController(
            ICommandRepository commandRepository,
            ICartItemRepository cartItemRepository,
            IOrderDetailRepository orderDetailRepository,
            IMapper mapper,
            IConfiguration configuration,
            IUserRepository userRepository,
            IEmailService emailService)
            : base(commandRepository, mapper)
        {
            _commandRepository = commandRepository;
            _cartItemRepository = cartItemRepository;
            _orderDetailRepository = orderDetailRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
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
        [HttpGet("{id:int}")]
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

        /// <summary>
        /// Validates a payment session and creates a command then sends an e-mail to the customer.
        /// </summary>
        [HttpPost("validate-payment")]
        public async Task<IActionResult> ValidatePayment([FromQuery] string sessionId)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            try
            {
                Stripe.StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                if (session.PaymentStatus != "paid") return BadRequest("Paiement non validé.");

                var cartItems = await _cartItemRepository.GetByUserIdAsync(userId);
                if (!cartItems.Any()) return Ok(new { message = "Commande déjà traitée." });

                Command command = new Command
                {
                    UserId = userId,
                    CommandStatusId = 2,
                    Date = DateTime.UtcNow,
                    Total = (decimal)(session.AmountTotal ?? 0) / 100m,
                    PdfName = $"Facture_{DateTime.Now:yyyyMMdd}_{userId}.pdf",
                    PdfPath = "/content/invoices/placeholder.pdf"
                };

                await _repository.AddAsync(command);

                List<OrderDetail> orderDetails = new List<OrderDetail>();
                StringBuilder rowsHtml = new StringBuilder();

                foreach (var item in cartItems)
                {
                    orderDetails.Add(new OrderDetail
                    {
                        CommandId = command.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });

                    string pName = item.ProductNavigation?.Label ?? "Produit Inconnu";
                    decimal pPrice = item.ProductNavigation?.Price ?? 0;
                    decimal lineTotal = pPrice * item.Quantity;
                    string imgUrl = item.ProductNavigation?.ProductPictureUrl;

                    rowsHtml.Append($@"
                <tr style='border-bottom: 1px solid #4a4e69;'>
                    <td style='padding: 12px; width: 60px;'>
                        <img src='{imgUrl}' alt='Img' style='width: 50px; height: 50px; object-fit: cover; border-radius: 6px; border: 1px solid #4a4e69;' />
                    </td>
                    <td style='padding: 12px; color: #e0e0e0; vertical-align: middle;'>
                        <span style='font-size: 14px; font-weight: bold;'>{pName}</span>
                    </td>
                    <td style='padding: 12px; text-align: center; color: #e0e0e0; vertical-align: middle;'>
                        x{item.Quantity}
                    </td>
                    <td style='padding: 12px; text-align: right; font-weight: bold; color: white; vertical-align: middle;'>
                        {lineTotal:F2} €
                    </td>
                </tr>");
                }

                await _orderDetailRepository.AddRangeAsync(orderDetails);
                await _cartItemRepository.ClearCartAsync(userId);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    try
                    {
                        string subject = $"Confirmation de commande #{command.Id}";

                        string message = $@"
                <div style='background-color: #0f0c29; color: white; font-family: Segoe UI, sans-serif; padding: 40px;'>
                    
                    <div style='text-align: center; margin-bottom: 30px;'>
                        <h1 style='color: #a29bfe; margin: 0; letter-spacing: 2px;'>ASTRALIS</h1>
                        <p style='color: #b2bec3; font-size: 14px; text-transform: uppercase;'>Mission Confirmée</p>
                    </div>

                    <div style='background-color: #1a1a40; padding: 25px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.3);'>
                        <p style='margin-top: 0; font-size: 16px; color: #dfe6e9;'>Bonjour <strong>{user.Username}</strong>,</p>
                        <p style='color: #b2bec3; margin-bottom: 20px;'>Vos équipements sont validés. Voici le manifeste de chargement :</p>
                        
                        <table style='width: 100%; border-collapse: collapse; font-size: 14px;'>
                            <thead>
                                <tr style='color: #a29bfe; font-size: 12px; text-transform: uppercase; letter-spacing: 1px;'>
                                    <th style='padding: 10px; text-align: left;' colspan='2'>Équipement</th>
                                    <th style='padding: 10px; text-align: center;'>Qté</th>
                                    <th style='padding: 10px; text-align: right;'>Total</th>
                                </tr>
                            </thead>
                            <tbody>
                                {rowsHtml}
                            </tbody>
                        </table>

                        <div style='text-align: right; margin-top: 25px; padding-top: 15px; border-top: 1px solid #4a4e69;'>
                            <p style='font-size: 14px; color: #b2bec3; margin: 0;'>Total payé</p>
                            <p style='font-size: 24px; margin: 5px 0 0 0; color: #00cec9; font-weight: bold;'>{command.Total:F2} €</p>
                        </div>
                    </div>

                    <div style='text-align: center; margin-top: 40px; color: #636e72; font-size: 12px;'>
                        <p>Merci de voyager avec Astralis.</p>
                    </div>
                </div>";

                        await _emailService.SendEmailAsync(user.Email, subject, message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur Mail: {ex.Message}");
                    }
                }

                return Ok(new { commandId = command.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Validates a checkout and creates a new command.
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
        [HttpPut("{id:int}")] // CORRECTION: ajout de :int
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
        [HttpDelete("{id:int}")] // CORRECTION: ajout de :int
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