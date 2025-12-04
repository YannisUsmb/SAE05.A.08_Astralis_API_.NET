using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DisplayName("EventType")]
    public class EventTypesController : ReadableController<EventType, EventTypeDto, EventTypeDto, int>
    {
        public EventTypesController(IEventTypeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}