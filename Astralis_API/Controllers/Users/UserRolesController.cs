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
    [DisplayName("UserRole")]
    public class UserRolesController : ReadableController<UserRole, UserRoleDto, UserRoleDto, int>
    {
        public UserRolesController(IUserRoleRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}