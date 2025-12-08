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
    [DisplayName("Planet Type")]
    public class PlanetTypesController : ReadableController<PlanetType, PlanetTypeDto, PlanetTypeDto, int>
    {
        public PlanetTypesController(IPlanetTypeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}