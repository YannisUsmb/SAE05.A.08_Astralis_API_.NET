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
    [DisplayName("Alias Status")]
    public class AliasStatusesController : ReadableController<AliasStatus, AliasStatusDto, AliasStatusDto, int>
    {
        public AliasStatusesController(IAliasStatusRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}