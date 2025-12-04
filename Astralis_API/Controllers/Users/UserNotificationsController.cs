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
    [DisplayName("UserNotification")]
    public class UserNotificationsController : JoinController<UserNotification, UserNotificationDto, UserNotificationCreateDto, int, int>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;

        public UserNotificationsController(IUserNotificationRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _userNotificationRepository = repository;
        }

        /// <summary>
        /// Retrieves all notifications for the currently authenticated user.
        /// </summary>
        /// <returns>A list of the user's notifications.</returns>
        /// <response code="200">Returns the list of notifications.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<IEnumerable<UserNotificationDto>>> GetAll()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            IEnumerable<UserNotification?> myNotifications = await _userNotificationRepository.GetByUserIdAsync(userId);

            return Ok(_mapper.Map<IEnumerable<UserNotificationDto>>(myNotifications));
        }

        /// <summary>
        /// Updates the read status of a notification.
        /// </summary>
        /// <param name="userId">The User ID (must match authenticated user).</param>
        /// <param name="notificationId">The Notification ID.</param>
        /// <param name="updateDto">The update details.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Status updated successfully.</response>
        /// <response code="400">ID mismatch.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User tried to update someone else's notification.</response>
        /// <response code="404">Notification not found.</response>
        [HttpPut("{userId}/{notificationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int userId, int notificationId, UserNotificationUpdateDto updateDto)
        {
            if (userId != updateDto.UserId || notificationId != updateDto.NotificationId)
            {
                return BadRequest("URL IDs do not match body IDs.");
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdString, out int currentUserId) || userId != currentUserId)
            {
                return Forbid();
            }

            UserNotification? entity = await _repository.GetByIdAsync(userId, notificationId);
            if (entity == null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, entity);
            await _repository.UpdateAsync(entity, entity);

            return NoContent();
        }
    }
}