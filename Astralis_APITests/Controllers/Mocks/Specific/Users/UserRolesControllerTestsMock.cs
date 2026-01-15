using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class UserRolesControllerTestsMock : ReadableControllerMockTests<UserRolesController, UserRole, UserRoleDto, UserRoleDto, int>
    {
        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
        }

        protected override UserRolesController CreateController(Mock<IReadableRepository<UserRole, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            var mockUserRoleRepo = new Mock<IUserRoleRepository>();

            _mockRepository = mockUserRoleRepo.As<IReadableRepository<UserRole, int>>();

            return new UserRolesController(mockUserRoleRepo.Object, mapper);
        }

        protected override List<UserRole> GetSampleEntities() => new List<UserRole>
        {
            new UserRole { Id = 1, Label = "Administrateur" },
            new UserRole { Id = 2, Label = "Rédacteur" },
            new UserRole { Id = 3, Label = "Utilisateur" }
        };

        protected override UserRole GetSampleEntity() => new UserRole
        {
            Id = 1,
            Label = "Administrateur"
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 99;
    }
}