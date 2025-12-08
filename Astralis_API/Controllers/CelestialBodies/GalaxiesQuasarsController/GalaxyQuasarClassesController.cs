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
    [DisplayName("Galaxy Quasar Class")]
    public class GalaxyQuasarClassesController : ReadableController<GalaxyQuasarClass, GalaxyQuasarClassDto, GalaxyQuasarClassDto, int>
    {
        public GalaxyQuasarClassesController(IGalaxyQuasarClassRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}