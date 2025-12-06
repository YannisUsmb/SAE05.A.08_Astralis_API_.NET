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
    [DisplayName("User Notification Type")]
    public class UserNotificationTypesController : JoinController<UserNotificationType, UserNotificationTypeDto, UserNotificationTypeCreateDto, int, int>
    {
        private readonly IUserNotificationTypeRepository _userNotificationTypeRepository;

        public UserNotificationTypesController(IUserNotificationTypeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _userNotificationTypeRepository = repository;
        }

        /// <summary>
        /// Retrieves notification preferences for the authenticated user.
        /// </summary>
        /// <returns>A list of notification preferences.</returns>
        /// <response code="200">Preferences retrieved successfully.</response>
        /// <response code="401">User not authenticated.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public override async Task<ActionResult<IEnumerable<UserNotificationTypeDto>>> GetAll()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            IEnumerable<UserNotificationType?> myPrefs = await _userNotificationTypeRepository.GetByUserIdAsync(userId);

            return Ok(_mapper.Map<IEnumerable<UserNotificationTypeDto>>(myPrefs));
        }

        /// <summary>
        /// Subscribes the user to a new notification type.
        /// </summary>
        /// <param name="createDto">Subscription details.</param>
        /// <returns>Status 200 OK.</returns>
        /// <response code="200">Subscription successful.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="401">User not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public override async Task<ActionResult> Post(UserNotificationTypeCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            UserNotificationType entity = _mapper.Map<UserNotificationType>(createDto);
            entity.UserId = userId;

            await _repository.AddAsync(entity);
            return Ok();
        }

        /// <summary>
        /// Updates a notification preference (e.g. enable/disable email).
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="notificationTypeId">Notification Type ID.</param>
        /// <param name="updateDto">Update details.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Preference updated.</response>
        /// <response code="400">ID mismatch.</response>
        /// <response code="403">Forbidden access.</response>
        /// <response code="404">Preference not found.</response>
        [HttpPut("{userId}/{notificationTypeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int userId, int notificationTypeId, UserNotificationTypeUpdateDto updateDto)
        {
            if (notificationTypeId != updateDto.NotificationTypeId)
            {
                return BadRequest("NotificationType ID mismatch.");
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdString, out int currentUserId) || userId != currentUserId)
            {
                return Forbid();
            }

            UserNotificationType? entity = await _repository.GetByIdAsync(userId, notificationTypeId);
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