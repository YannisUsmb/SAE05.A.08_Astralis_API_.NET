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
    [DisplayName("Orbial Class")]
    public class OrbitalClassesController : ReadableController<OrbitalClass, OrbitalClassDto, OrbitalClassDto, int>
    {
        public OrbitalClassesController(IOrbitalClassRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}