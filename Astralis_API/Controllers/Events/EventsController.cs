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
    [DisplayName("Event")]
    public class EventsController : CrudController<Event, AddressDto, EventDto, EventCreateDto, EventUpdateDto, int>
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _eventRepository = repository;
        }

        /// <summary>
        /// Searches for events using multiple criteria (text, type, dates).
        /// </summary>
        /// <param name="search">Search filters passed as query parameters.</param>
        /// <returns>A list of matching events.</returns>
        /// <response code="200">List of events retrieved successfully.</response>
        [HttpGet("Search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventDto>>> Search([FromQuery] EventFilterDto search)
        {
            IEnumerable<Event?> events = await _eventRepository.SearchAsync(
                search.SearchText,
                search.EventTypeIds,
                search.MinStartDate,
                search.MaxStartDate,
                search.MinEndDate,
                search.MaxEndDate
            );

            return Ok(_mapper.Map<IEnumerable<EventDto>>(events));
        }
    }
}