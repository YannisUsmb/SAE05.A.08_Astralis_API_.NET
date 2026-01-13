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
    [DisplayName("Event Type")]
    public class EventTypesController : CrudController<EventType, EventTypeDto, EventTypeDto, EventTypeCreateDto, EventTypeUpdateDto, int>
    {
        public EventTypesController(IEventTypeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        [HttpPost]
        [Authorize(Roles = "Rédacteur Commercial, Admin")]
        public override async Task<ActionResult<EventTypeDto>> Post(EventTypeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<EventType>(dto);
            await _repository.AddAsync(entity);

            var newDto = _mapper.Map<EventTypeDto>(entity);
            return Ok(newDto);
        }
    }
}