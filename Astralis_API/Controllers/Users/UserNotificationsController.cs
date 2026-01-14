using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace Astralis_API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user-specific notifications.
    /// Handles creation (with email dispatch), retrieval, status updates (read/unread), and deletion.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("User Notification")]
    public class UserNotificationsController : CrudController<UserNotification, UserNotificationDto, UserNotificationDto, UserNotificationCreateDto, UserNotificationUpdateDto, int>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserNotificationTypeRepository _userNotificationTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationsController"/> class.
        /// </summary>
        /// <param name="repository">The main repository for user notifications.</param>
        /// <param name="notificationRepository">Repository to access notification definitions.</param>
        /// <param name="userNotificationTypeRepository">Repository to check user preferences.</param>
        /// <param name="userRepository">Repository to access user emails.</param>
        /// <param name="emailService">Service to send emails.</param>
        /// <param name="mapper">AutoMapper instance.</param>
        public UserNotificationsController(
            IUserNotificationRepository repository,
            INotificationRepository notificationRepository,
            IUserNotificationTypeRepository userNotificationTypeRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IMapper mapper)
            : base(repository, mapper)
        {
            _userNotificationRepository = repository;
            _notificationRepository = notificationRepository;
            _userNotificationTypeRepository = userNotificationTypeRepository;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        /// <summary>
        /// Creates a new notification association for a user.
        /// Triggers an email if the user has enabled email notifications for this specific type.
        /// </summary>
        /// <param name="createDto">The notification creation details.</param>
        /// <returns>The created notification object.</returns>
        /// <response code="200">The notification was successfully created.</response>
        /// <response code="400">The input data is invalid.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<UserNotificationDto>> Post(UserNotificationCreateDto createDto)
        {
            var result = await base.Post(createDto);

            if (result.Result is not OkObjectResult okResult || okResult.Value is not UserNotificationDto)
            {
                return result;
            }

            try
            {
                await CheckAndSendEmailAsync(createDto.UserId, createDto.NotificationId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification email: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Retrieves all notifications for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of notifications belonging to the user.</returns>
        /// <response code="200">The list was retrieved successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is attempting to view another user's notifications without Admin privileges.</response>
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
        /// Updates a specific user notification (e.g. marking it as read).
        /// </summary>
        /// <param name="id">The unique identifier of the user notification (PK).</param>
        /// <param name="updateDto">The object containing the updated values.</param>
        /// <returns>The updated notification object.</returns>
        /// <response code="200">The notification was successfully updated.</response>
        /// <response code="400">ID mismatch between URL and body.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is attempting to update a notification that does not belong to them.</response>
        /// <response code="404">Notification not found.</response>
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
        /// <response code="404">Notification not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        /// <summary>
        /// Helper method to check user preferences and send an email if applicable.
        /// </summary>
        private async Task CheckAndSendEmailAsync(int userId, int notificationId)
        {
            var notificationDef = await _notificationRepository.GetByIdAsync(notificationId);
            if (notificationDef == null) return;

            var userPreference = await _userNotificationTypeRepository.GetByIdAsync(userId, notificationDef.NotificationTypeId);

            if (userPreference != null && userPreference.ByMail)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.Email)) return;

                string subject = $"Astralis - {notificationDef.Label}";
                string message = $@"
                <div style='background-color: #0f1729; color: #e4e4e7; padding: 30px; font-family: sans-serif; border-radius: 8px;'>
                    <h2 style='color: #a76dff; margin-bottom: 20px;'>Nouvelle Notification</h2>
                    <div style='background-color: rgba(255, 255, 255, 0.05); padding: 20px; border-radius: 6px; border: 1px solid rgba(255, 255, 255, 0.1);'>
                        <h3 style='margin-top: 0; color: #fff;'>{notificationDef.Label}</h3>
                        <p style='font-size: 16px; line-height: 1.5;'>{notificationDef.Description}</p>
                        
                        {(string.IsNullOrEmpty(notificationDef.Link) ? "" :
                            $"<br/><div style='text-align: center; margin-top: 20px;'><a href='https://localhost:7036{notificationDef.Link}' style='background-color: #7f5af0; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Voir sur Astralis</a></div>"
                        )}
                    </div>
                    <p style='font-size: 12px; color: #666; margin-top: 30px; text-align: center;'>
                        Vous recevez cet email car vous avez activé les notifications pour ce type.
                        <br/>
                        <a href='https://localhost:7036/parametres/preferences' style='color: #666; text-decoration: underline;'>Modifier mes préférences</a>
                    </p>
                </div>";

                await _emailService.SendEmailAsync(user.Email, subject, message);
            }
        }
    }
}