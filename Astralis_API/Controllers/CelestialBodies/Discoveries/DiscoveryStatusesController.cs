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
    [DisplayName("Discovery Status")]
    public class DiscoveryStatusesController : ReadableController<DiscoveryStatus, DiscoveryStatusDto, DiscoveryStatusDto, int>
    {
        public DiscoveryStatusesController(IDiscoveryStatusRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}