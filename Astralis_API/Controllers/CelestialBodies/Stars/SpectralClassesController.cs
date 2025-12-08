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
    [DisplayName("Spectral Class")]
    public class SpectralClassesController : ReadableController<SpectralClass, SpectralClassDto, SpectralClassDto, int>
    {
        public SpectralClassesController(ISpectralClassRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}