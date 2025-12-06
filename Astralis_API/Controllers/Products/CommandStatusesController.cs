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
    [DisplayName("Command Status")]
    public class CommandStatusesController : ReadableController<CommandStatus, CommandStatusDto, ProductCategoryDto, int>
    {
        public CommandStatusesController(ICommandStatusRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}