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
    /// <summary>
    /// Controller responsible for managing specific user notifications.
    /// Handles retrieval per user, updating read status, and deletion.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("User Notification")]
    public class UserNotificationsController : CrudController<UserNotification, UserNotificationDto, UserNotificationDto, UserNotificationCreateDto, UserNotificationUpdateDto, int>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationsController"/> class.
        /// </summary>
        /// <param name="repository">The user notification repository.</param>
        /// <param name="mapper">The auto-mapper instance.</param>
        public UserNotificationsController(IUserNotificationRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _userNotificationRepository = repository;
        }

        /// <summary>
        /// Retrieves all notifications for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A collection of notifications enriched with details (title, description, link).</returns>
        /// <response code="200">The notifications were successfully retrieved.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is attempting to access another user's notifications without Admin privileges.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("{userId}/notifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserNotificationDto>>> GetMyNotifications(int userId)
        {
            string? tokenUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(tokenUserIdStr, out int tokenUserId) || (userId != tokenUserId && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            var notifications = await _userNotificationRepository.GetByUserIdAsync(userId);
            return Ok(_mapper.Map<IEnumerable<UserNotificationDto>>(notifications));
        }

        /// <summary>
        /// Updates a specific user notification (e.g., to mark it as read).
        /// </summary>
        /// <param name="id">The unique identifier of the user notification (Primary Key).</param>
        /// <param name="updateDto">The object containing the updated values.</param>
        /// <returns>The updated user notification object.</returns>
        /// <response code="200">The notification was successfully updated.</response>
        /// <response code="400">The ID in the URL does not match the ID in the body.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is attempting to update a notification that does not belong to them.</response>
        /// <response code="404">The notification with the specified ID was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, UserNotificationUpdateDto updateDto)
        {
            var entity = await _userNotificationRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            string? tokenUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(tokenUserIdStr, out int tokenUserId) && entity.UserId != tokenUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _mapper.Map(updateDto, entity);
            await _userNotificationRepository.UpdateAsync(entity, entity);

            return Ok(_mapper.Map<UserNotificationDto>(entity));
        }

        /// <summary>
        /// Deletes a specific user notification.
        /// </summary>
        /// <param name="id">The unique identifier of the user notification to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The notification was successfully deleted.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is attempting to delete a notification that does not belong to them.</response>
        /// <response code="404">The notification was not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            var entity = await _userNotificationRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            string? tokenUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(tokenUserIdStr, out int tokenUserId) && entity.UserId != tokenUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _userNotificationRepository.DeleteAsync(entity);
            return NoContent();
        }
    }
}