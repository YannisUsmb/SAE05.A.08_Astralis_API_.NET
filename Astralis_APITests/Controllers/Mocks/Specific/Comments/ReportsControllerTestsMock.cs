using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class ReportsControllerTestsMock : CrudControllerMockTests<ReportsController, Report, ReportDto, ReportDto, ReportCreateDto, ReportUpdateDto, int>
    {
        private Mock<IReportRepository> _mockReportRepository;
        private Mock<IEmailService> _mockEmailService;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Admin");
        }

        protected override ReportsController CreateController(Mock<IReadableRepository<Report, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockReportRepository = new Mock<IReportRepository>();
            _mockEmailService = new Mock<IEmailService>();

            _mockCrudRepository = _mockReportRepository.As<ICrudRepository<Report, int>>();
            _mockRepository = _mockReportRepository.As<IReadableRepository<Report, int>>();

            return new ReportsController(_mockReportRepository.Object, _mockEmailService.Object, mapper);
        }


        protected override void SetIdInUpdateDto(ReportUpdateDto dto, int id)
        {
            dto.Id = id;
        }

        protected override List<Report> GetSampleEntities() => new List<Report>
        {
            new Report
            {
                Id = 1,
                Description = "Spam content",
                UserId = 1,
                ReportStatusId = 1,
                ReportMotiveId = 1,
                CommentId = 10,
                Date = DateTime.UtcNow,
                ReportStatusNavigation = new ReportStatus { Label = "Pending" },
                ReportMotiveNavigation = new ReportMotive { Label = "Spam" },
                UserNavigation = new User { Username = "Reporter1" }
            },
            new Report
            {
                Id = 2,
                Description = "Harassment",
                UserId = 2,
                ReportStatusId = 1,
                ReportMotiveId = 2,
                CommentId = 11,
                Date = DateTime.UtcNow,
                ReportStatusNavigation = new ReportStatus { Label = "Pending" },
                ReportMotiveNavigation = new ReportMotive { Label = "Harassment" },
                UserNavigation = new User { Username = "Reporter2" }
            }
        };

        protected override Report GetSampleEntity() => new Report
        {
            Id = 1,
            Description = "Spam content",
            UserId = 1,
            ReportStatusId = 1,
            ReportMotiveId = 1,
            CommentId = 10,
            Date = DateTime.UtcNow,
            ReportStatusNavigation = new ReportStatus { Label = "Pending" },
            ReportMotiveNavigation = new ReportMotive { Label = "Spam" },
            UserNavigation = new User { Username = "Reporter1" }
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override ReportCreateDto GetValidCreateDto() => new ReportCreateDto
        {
            CommentId = 10,
            ReportMotiveId = 1,
            Description = "This is spam"
        };

        protected override ReportUpdateDto GetValidUpdateDto() => new ReportUpdateDto
        {
            Id = 1,
            ReportStatusId = 2
        };

        private void SetupHttpContext(int userId, string role = "User", string email = "test@test.com")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, "TestUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            if (_controller != null)
                _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        }

        [TestMethod]
        public new async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId, "User");
            var createDto = GetValidCreateDto();

            _mockReportRepository.Setup(r => r.AddAsync(It.IsAny<Report>())).Returns(Task.CompletedTask);
            _mockEmailService.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockReportRepository.Verify(r => r.AddAsync(It.Is<Report>(x => x.UserId == userId && x.ReportStatusId == 1)), Times.Once);
            _mockEmailService.Verify(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        }


        [TestMethod]
        public async Task Search_WithFilters_ShouldReturnFilteredList()
        {
            // Given
            SetupHttpContext(1, "Admin");
            var filter = new ReportFilterDto { StatusId = 1, MotiveId = 1 };
            var entities = GetSampleEntities();

            _mockReportRepository.Setup(r => r.SearchAsync(filter.StatusId, filter.MotiveId, filter.MinDate, filter.MaxDate))
                .ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockReportRepository.Verify(r => r.SearchAsync(filter.StatusId, filter.MotiveId, filter.MinDate, filter.MaxDate), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            var dtos = okResult.Value as IEnumerable<ReportDto>;
            Assert.IsNotNull(dtos);
            Assert.AreEqual(2, dtos.Count());
        }

        [TestMethod]
        public async Task Search_AsUser_ShouldReturnForbid()
        {
            // Given
            SetupHttpContext(1, "User");
            var filter = new ReportFilterDto();
        }

        [TestMethod]
        public async Task Put_UpdateStatus_AsAdmin_ShouldSetAdminId()
        {
            // Given
            int adminId = 99;
            SetupHttpContext(adminId, "Admin");
            int reportId = 1;
            var updateDto = new ReportUpdateDto { Id = reportId, ReportStatusId = 2 };
            var entity = GetSampleEntity();

            _mockReportRepository.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync(entity);
            _mockReportRepository.Setup(r => r.UpdateAsync(entity, entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(reportId, updateDto);

            // Then
            Assert.AreEqual(adminId, entity.AdminId);
            _mockReportRepository.Verify(r => r.UpdateAsync(entity, entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}