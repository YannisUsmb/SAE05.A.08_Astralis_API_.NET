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
    [DisplayName("Event")]
    public class EventsController : CrudController<Event, EventDto, EventDto, EventCreateDto, EventUpdateDto, int>
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _eventRepository = repository;
        }

        /// <summary>
        /// Retrieves all events (public access).
        /// </summary>
        /// <returns>A list of events.</returns>
        /// <response code="200">Returns the list of events.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<EventDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific event by ID (public access).
        /// </summary>
        /// <param name="id">The event ID.</param>
        /// <returns>The detailed event.</returns>
        /// <response code="200">Returns the event.</response>
        /// <response code="404">Event not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<EventDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for events using multiple criteria (text, type, dates).
        /// </summary>
        /// <param name="search">Search filters passed as query parameters.</param>
        /// <returns>A list of matching events.</returns>
        /// <response code="200">List of events retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EventDto>>> Search([FromQuery] EventFilterDto search)
        {
            IEnumerable<Event> events = await _eventRepository.SearchAsync(
                search.SearchText,
                search.EventTypeIds,
                search.MinStartDate,
                search.MaxStartDate,
                search.MinEndDate,
                search.MaxEndDate
            );

            return Ok(_mapper.Map<IEnumerable<EventDto>>(events));
        }

        /// <summary>
        /// Creates a new event (Commercial Editors only).
        /// </summary>
        /// <param name="createDto">The event details.</param>
        /// <returns>The created event.</returns>
        /// <response code="200">Event successfully created.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User not authorized (requires CommercialEditor role).</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize(Roles = "Rédacteur Commercial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<EventDto>> Post(EventCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            Event entity = _mapper.Map<Event>(createDto);
            entity.UserId = userId;

            await _repository.AddAsync(entity);
            return Ok(_mapper.Map<EventDto>(entity));
        }

        /// <summary>
        /// Updates an existing event (Commercial Editors only).
        /// </summary>
        /// <remarks>Commercial Editors can modify any event, or you can restrict to their own events here.</remarks>
        /// <param name="id">Event ID.</param>
        /// <param name="updateDto">Updated details.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Event updated successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User not authorized (requires CommercialEditor role).</response>
        /// <response code="404">Event not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Rédacteur Commercial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, EventUpdateDto updateDto)
        {
            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes an event (Commercial Editors only).
        /// </summary>
        /// <param name="id">The unique identifier of the Event to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The event was successfully deleted.</response>
        /// <response code="404">The event does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            Event? entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || entity.UserId != userId)
            {
                return Forbid();
            }

            return await base.Delete(id);
        }
    }
}