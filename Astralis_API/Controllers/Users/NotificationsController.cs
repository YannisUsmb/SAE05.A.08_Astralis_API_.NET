using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("Notification")]
    public class NotificationsController : CrudController<Notification, NotificationDto, NotificationDto, NotificationCreateDto, NotificationUpdateDto, int>
    {
        public NotificationsController(INotificationRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        /// <summary>
        /// Retrieves all notification definitions.
        /// </summary>
        /// <returns>A list of notification definitions.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific notification definition.
        /// </summary>
        /// <returns>A list of notification definitions.</returns>
        /// <param name="id">Notification ID.</param>
        /// <returns>The notification definition.</returns>
        /// <response code="200">Notification retrieved successfully.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<NotificationDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Creates a new notification definition (Admin only).
        /// </summary>
        /// <param name="createDto">The notification details.</param>
        /// <returns>The created notification.</returns>
        /// <response code="200">Notification successfully created.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User not authorized (requires Admin role).</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public override Task<ActionResult<NotificationDto>> Post(NotificationCreateDto createDto)
        {
            return base.Post(createDto);
        }

        /// <summary>
        /// Updates a notification definition (Admin only).
        /// </summary>
        /// <param name="id">Notification ID.</param>
        /// <param name="updateDto">Updated details.</param>
        /// <returns>No content.</returns>
        /// <response code="200">Notification successfully updated.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User not authorized (requires Admin role).</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public override Task<IActionResult> Put(int id, NotificationUpdateDto updateDto)
        {
            return base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a notification (admin only).
        /// </summary>
        /// <param name="id">The unique identifier of the notification to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The notification was successfully deleted.</response>
        /// <response code="404">The notification does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<IActionResult> Delete(int id)
        {
            return base.Delete(id);
        }
    }
}