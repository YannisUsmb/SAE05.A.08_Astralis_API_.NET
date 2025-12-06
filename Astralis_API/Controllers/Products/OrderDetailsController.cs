using Astralis.Shared.DTOs;
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
    [DisplayName("Order Detail")]
    public class OrderDetailsController : JoinController<OrderDetail, OrderDetailDto, OrderDetailCreateDto, int, int>
    {
        private readonly ICommandRepository _commandRepository;

        public OrderDetailsController(
            IOrderDetailRepository repository,
            ICommandRepository commandRepository,
            IMapper mapper)
            : base(repository, mapper)
        {
            _commandRepository = commandRepository;
        }

        /// <summary>
        /// Retrieves a specific line item from an order.
        /// </summary>
        /// <param name="id1">The Command ID.</param>
        /// <param name="id2">The Product ID.</param>
        /// <returns>The requested order detail.</returns>
        /// <response code="200">Returns the order detail.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to view this order.</response>
        /// <response code="404">Order detail not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id1}/{id2}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<OrderDetailDto>> GetById(int id1, int id2)
        {
            var entity = await _repository.GetByIdAsync(id1, id2);
            if (entity == null) return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            if (userRole != "Admin" && entity.CommandNavigation.UserId != userId)
            {
                return Forbid();
            }

            return Ok(_mapper.Map<OrderDetailDto>(entity));
        }

        /// <summary>
        /// Adds a product line to an existing order manually.
        /// </summary>
        /// <remarks>
        /// This endpoint is rarely used directly by clients (who use Checkout), 
        /// but can be useful for Admins or specific adjustments.
        /// The order must be in 'Pending' status.
        /// </remarks>
        /// <param name="createDto">The details of the line to add.</param>
        /// <returns>Status 200 OK.</returns>
        /// <response code="200">Item added successfully.</response>
        /// <response code="400">Invalid input or Order is already processed.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to modify this order.</response>
        /// <response code="404">Parent Order not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult> Post(OrderDetailCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var command = await _commandRepository.GetByIdAsync(createDto.CommandId);
            if (command == null) return NotFound("Command not found.");

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            if (userRole != "Admin" && command.UserId != userId)
            {
                return Forbid();
            }

            if (command.CommandStatusId != 1)
            {
                return BadRequest("Cannot add items to a processed order.");
            }

            return await base.Post(createDto);
        }

        /// <summary>
        /// Updates the quantity of a product in an order.
        /// </summary>
        /// <param name="commandId">The Command ID.</param>
        /// <param name="productId">The Product ID.</param>
        /// <param name="updateDto">The update details (Quantity).</param>
        /// <returns>No content.</returns>
        /// <response code="204">Quantity updated successfully.</response>
        /// <response code="400">Invalid input, ID mismatch, or Order is already processed.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to modify this order.</response>
        /// <response code="404">Order detail not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{commandId}/{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int commandId, int productId, OrderDetailUpdateDto updateDto)
        {
            if (commandId != updateDto.CommandId || productId != updateDto.ProductId)
                return BadRequest("Ids match error.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _repository.GetByIdAsync(commandId, productId);
            if (entity == null) return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            if (userRole != "Admin" && entity.CommandNavigation.UserId != userId)
            {
                return Forbid();
            }

            if (entity.CommandNavigation.CommandStatusId != 1 && entity.CommandNavigation.CommandStatusId != 5)
            {
                return BadRequest("Cannot modify a processed order.");
            }

            _mapper.Map(updateDto, entity);
            await _repository.UpdateAsync(entity, entity);

            return NoContent();
        }

        /// <summary>
        /// Removes a product line from an order.
        /// </summary>
        /// <param name="id1">The Command ID.</param>
        /// <param name="id2">The Product ID.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Item removed successfully.</response>
        /// <response code="400">Order is already processed.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to modify this order.</response>
        /// <response code="404">Order detail not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id1}/{id2}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id1, int id2)
        {
            var entity = await _repository.GetByIdAsync(id1, id2);
            if (entity == null) return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            if (userRole != "Admin" && entity.CommandNavigation.UserId != userId)
            {
                return Forbid();
            }

            if (entity.CommandNavigation.CommandStatusId != 1 && entity.CommandNavigation.CommandStatusId != 5)
            {
                return BadRequest("Cannot delete items from a processed order.");
            }

            return await base.Delete(id1, id2);
        }
    }
}