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
    [DisplayName("Detection Method")]
    public class DetectionMethodsController : ReadableController<DetectionMethod, DetectionMethodDto, DetectionMethodDto, int>
    {
        public DetectionMethodsController(IDetectionMethodRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}