using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class CommandStatusesControllerTestsMock : ReadableControllerMockTests<CommandStatusesController, CommandStatus, CommandStatusDto, CommandStatusDto, int>
    {
        private Mock<ICommandStatusRepository> _mockCommandStatusRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override CommandStatusesController CreateController(Mock<IReadableRepository<CommandStatus, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            // Given
            _mockCommandStatusRepository = new Mock<ICommandStatusRepository>();

            // When
            _mockRepository = _mockCommandStatusRepository.As<IReadableRepository<CommandStatus, int>>();

            // Then
            return new CommandStatusesController(_mockCommandStatusRepository.Object, mapper);
        }

        protected override List<CommandStatus> GetSampleEntities() => new List<CommandStatus>
        {
            new CommandStatus { Id = 1, Label = "Pending" },
            new CommandStatus { Id = 2, Label = "Validated" }
        };

        protected override CommandStatus GetSampleEntity() => new CommandStatus
        {
            Id = 1,
            Label = "Pending"
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        private void SetupHttpContext(int userId, string role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            if (_controller != null)
                _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        }
    }
}