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
    [DisplayName("Event Interest")]
    public class EventInterestsController : JoinController<EventInterest, EventInterestDto, EventInterestDto, int, int>
    {
        public EventInterestsController(IEventInterestRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}