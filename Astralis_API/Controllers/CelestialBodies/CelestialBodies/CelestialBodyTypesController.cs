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
    [DisplayName("Celestial Body Type")]
    public class CelestialBodyTypesController : ReadableController<CelestialBodyType, CelestialBodyTypeDto, CelestialBodyTypeDto, int>
    {
        public CelestialBodyTypesController(ICelestialBodyTypeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}