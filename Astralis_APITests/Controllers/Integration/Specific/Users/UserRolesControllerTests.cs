using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class UserRolesControllerTests : ReadableControllerTests<UserRolesController, UserRole, UserRoleDto, UserRoleDto, int>
    {
        protected override UserRolesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new UserRolesController(new UserRoleManager(context), mapper);
        }

        protected override List<UserRole> GetSampleEntities()
        {
            return new List<UserRole>
            {
                new UserRole {Id=902101, Label = "UserRole 1"},
                new UserRole {Id=902102, Label = "UserRole 2"}
            };
        }

        protected override int GetIdFromEntity(UserRole entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}