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
    public class ReportMotivesControllerTestsMock : ReadableControllerMockTests<ReportMotivesController, ReportMotive, ReportMotiveDto, ReportMotiveDto, int>
    {
        private Mock<IReportMotiveRepository> _mockReportMotiveRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override ReportMotivesController CreateController(Mock<IReadableRepository<ReportMotive, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            // Given
            _mockReportMotiveRepository = new Mock<IReportMotiveRepository>();

            // When
            _mockRepository = _mockReportMotiveRepository.As<IReadableRepository<ReportMotive, int>>();

            // Then
            return new ReportMotivesController(_mockReportMotiveRepository.Object, mapper);
        }

        protected override List<ReportMotive> GetSampleEntities() => new List<ReportMotive>
        {
            new ReportMotive { Id = 1, Label = "Spam", Description = "Publicité ou contenu indésirable" },
            new ReportMotive { Id = 2, Label = "Harcèlement", Description = "Propos injurieux ou menaçants" }
        };

        protected override ReportMotive GetSampleEntity() => new ReportMotive
        {
            Id = 1,
            Label = "Spam",
            Description = "Publicité ou contenu indésirable"
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
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
                };
        }
    }
}