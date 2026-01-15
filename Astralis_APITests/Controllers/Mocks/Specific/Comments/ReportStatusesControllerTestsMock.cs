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
    public class ReportStatusesControllerTestsMock : ReadableControllerMockTests<ReportStatusesController, ReportStatus, ReportStatusDto, ReportStatusDto, int>
    {
        private Mock<IReportStatusRepository> _mockReportStatusRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override ReportStatusesController CreateController(Mock<IReadableRepository<ReportStatus, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            // Given
            _mockReportStatusRepository = new Mock<IReportStatusRepository>();

            // When
            _mockRepository = _mockReportStatusRepository.As<IReadableRepository<ReportStatus, int>>();

            // Then
            return new ReportStatusesController(_mockReportStatusRepository.Object, mapper);
        }

        protected override List<ReportStatus> GetSampleEntities() => new List<ReportStatus>
        {
            new ReportStatus { Id = 1, Label = "Pending" },
            new ReportStatus { Id = 2, Label = "Resolved" }
        };

        protected override ReportStatus GetSampleEntity() => new ReportStatus
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
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
                };
        }
    }
}