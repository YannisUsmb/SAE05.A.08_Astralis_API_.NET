using Astralis.Shared.DTOs;
using Astralis_API.Configuration;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("Cart Item")]
    public class CartItemsController : JoinController<CartItem, CartItemDto, CartItemCreateDto, int, int>
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemsController(ICartItemRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _cartItemRepository = repository;
        }

        /// <summary>
        /// Retrieves all items in the authenticated user's cart.
        /// </summary>
        /// <returns>A list of cart items.</returns>
        /// <response code="200">Returns the list of items.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<IEnumerable<CartItemDto>>> GetAll()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            IEnumerable<CartItem> items = await _cartItemRepository.GetByUserIdAsync(userId);
            return Ok(_mapper.Map<IEnumerable<CartItemDto>>(items));
        }

        /// <summary>
        /// Retrieves the full cart summary (items + grand total) for the authenticated user.
        /// </summary>
        /// <returns>The cart summary.</returns>
        /// <response code="200">Returns the cart summary.</response>
        /// <response code="401">User is not authenticated.</response>
        // --- MODIFICATION 1 : Renommé pour correspondre au Frontend (LoadCartAsync) ---
        [HttpGet("my-cart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartDto>> GetMyCart()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            IEnumerable<CartItem> items = await _cartItemRepository.GetByUserIdAsync(userId);

            CartDto cartDto = new CartDto
            {
                Items = _mapper.Map<List<CartItemDto>>(items)
            };

            return Ok(cartDto);
        }

        /// <summary>
        /// Retrieves a specific cart item.
        /// </summary>
        /// <param name="userId">User ID (must match authenticated user).</param>
        /// <param name="productId">Product ID.</param>
        /// <returns>The cart item.</returns>
        [HttpGet("{userId}/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult<CartItemDto>> GetById(int userId, int productId)
        {
            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdString, out int currentUserId))
            {
                return Unauthorized();
            }

            if (userId != currentUserId)
            {
                return Forbid();
            }

            return await base.GetById(userId, productId);
        }

        /// <summary>
        /// Adds an item to the cart or updates quantity if it already exists.
        /// </summary>
        /// <param name="createDto">Item details.</param>
        /// <returns>Status 200 OK with the object.</returns>
        /// <response code="200">Item added or updated successfully.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="401">User is not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult> Post(CartItemCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            CartItem? existingItem = await _cartItemRepository.GetByIdAsync(userId, createDto.ProductId);

            // --- MODIFICATION 2 : On capture l'item final pour le renvoyer ---
            CartItem itemResult;

            if (existingItem != null)
            {
                existingItem.Quantity += createDto.Quantity;
                await _cartItemRepository.UpdateAsync(existingItem, existingItem);
                itemResult = existingItem;
            }
            else
            {
                CartItem newItem = _mapper.Map<CartItem>(createDto);
                newItem.UserId = userId;
                await _cartItemRepository.AddAsync(newItem);
                itemResult = newItem;
            }

            // CORRECTION CRITIQUE : On retourne l'objet mappé pour que Blazor ne plante pas
            // (Erreur "input does not contain JSON tokens")
            return Ok(_mapper.Map<CartItemDto>(itemResult));
        }

        /// <summary>
        /// Updates the quantity of a cart item.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="productId">Product ID.</param>
        /// <param name="updateDto">New quantity.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The cart was successfully updated.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">The cart does not exist.</response>
        [HttpPut("{userId}/{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int userId, int productId, CartItemUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdString, out int currentUserId))
            {
                return Unauthorized();
            }

            if (userId != currentUserId)
            {
                return Forbid();
            }

            CartItem? existingItem = await _cartItemRepository.GetByIdAsync(userId, productId);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Quantity = updateDto.Quantity;
            await _cartItemRepository.UpdateAsync(existingItem, existingItem);

            return NoContent();
        }

        /// <summary>
        /// Removes an item from the cart.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="productId">Product ID.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The cart was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">The cart does not exist.</response>
        [HttpDelete("{userId}/{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> Delete(int userId, int productId)
        {
            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdString, out int currentUserId))
            {
                return Unauthorized();
            }

            if (userId != currentUserId)
            {
                return Forbid();
            }

            return await base.Delete(userId, productId);
        }

        // --- MODIFICATION 3 : Ajout de la méthode manquante pour vider le panier ---
        /// <summary>
        /// Clears all items from the user's cart.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>No content.</returns>
        [HttpDelete("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ClearCart(int userId)
        {
            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdString, out int currentUserId))
            {
                return Unauthorized();
            }

            if (userId != currentUserId)
            {
                return Forbid();
            }

            var items = await _cartItemRepository.GetByUserIdAsync(userId);
            foreach (var item in items)
            {
                await _cartItemRepository.DeleteAsync(item);
            }

            return NoContent();
        }
    }
}