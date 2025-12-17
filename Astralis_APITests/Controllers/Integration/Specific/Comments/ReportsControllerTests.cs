using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class ReportsControllerTests
        : CrudControllerTests<ReportsController, Report, ReportDto, ReportDto, ReportCreateDto, ReportUpdateDto, int>
    {
        private const int USER_NORMAL_ID = 5001;
        private const int USER_ADMIN_ID = 5002;

        private const int STATUS_PENDING_ID = 1;
        private const int STATUS_RESOLVED_ID = 2;
        private const int MOTIVE_SPAM_ID = 1;

        private const int ARTICLE_ID = 88001;
        private const int COMMENT_ID = 77001;

        private const int REPORT_ID_1 = 66001;
        private const int REPORT_ID_2 = 66002;

        private int _reportId1;
        private int _reportId2;

        protected override ReportsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var reportManager = new ReportManager(context);

            var controller = new ReportsController(
                reportManager,
                mapper
            );

            SetupUserContext(controller, USER_NORMAL_ID, "User");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, $"User_{userId}")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<Report> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            CreateUserIfNotExist(USER_NORMAL_ID, 1);
            CreateUserIfNotExist(USER_ADMIN_ID, 2);

            EnsureReportStatusExists(STATUS_PENDING_ID, "Pending");
            EnsureReportStatusExists(STATUS_RESOLVED_ID, "Resolved");

            EnsureReportMotiveExists(MOTIVE_SPAM_ID, "Spam");

            if (!_context.Articles.AsNoTracking().Any(a => a.Id == ARTICLE_ID))
            {
                _context.Articles.Add(new Article
                {
                    Id = ARTICLE_ID,
                    Title = "Controversial Article",
                    Content = "Content...",
                    UserId = USER_ADMIN_ID
                });
            }

            if (!_context.Comments.AsNoTracking().Any(c => c.Id == COMMENT_ID))
            {
                _context.Comments.Add(new Comment
                {
                    Id = COMMENT_ID,
                    UserId = USER_NORMAL_ID,
                    ArticleId = ARTICLE_ID,
                    Text = "This is a spam comment",
                    Date = DateTime.UtcNow,
                    IsVisible = true
                });
            }

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            var r1 = CreateReportInMemory(
                REPORT_ID_1,
                "Looks like spam",
                USER_NORMAL_ID,
                COMMENT_ID,
                MOTIVE_SPAM_ID,
                STATUS_PENDING_ID
            );

            var r2 = CreateReportInMemory(
                REPORT_ID_2,
                "Another spam",
                USER_NORMAL_ID,
                COMMENT_ID,
                MOTIVE_SPAM_ID,
                STATUS_RESOLVED_ID
            );

            _reportId1 = REPORT_ID_1;
            _reportId2 = REPORT_ID_2;

            return new List<Report> { r1, r2 };
        }

        private Report CreateReportInMemory(int id, string description, int userId, int commentId, int motiveId, int statusId)
        {
            return new Report
            {
                Id = id,
                Description = description,
                Date = DateTime.UtcNow,
                UserId = userId,
                CommentId = commentId,
                ReportMotiveId = motiveId,
                ReportStatusId = statusId,
                AdminId = null
            };
        }

        private void CreateUserIfNotExist(int id, int roleId)
        {
            if (!_context.Users.AsNoTracking().Any(u => u.Id == id))
            {
                _context.Users.Add(new User
                {
                    Id = id,
                    UserRoleId = roleId,
                    Username = $"User{id}",
                    Email = $"user{id}@test.com",
                    FirstName = "Test",
                    LastName = "User",
                    Password = "Pwd"
                });
            }
        }

        private void EnsureReportStatusExists(int id, string label)
        {
            if (!_context.ReportStatuses.AsNoTracking().Any(rs => rs.Id == id))
                _context.ReportStatuses.Add(new ReportStatus { Id = id, Label = label });
        }

        private void EnsureReportMotiveExists(int id, string label)
        {
            if (!_context.ReportMotives.AsNoTracking().Any(rm => rm.Id == id))
                _context.ReportMotives.Add(new ReportMotive { Id = id, Label = label });
        }

        protected override int GetIdFromEntity(Report entity) => entity.Id;
        protected override int GetIdFromDto(ReportDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override ReportCreateDto GetValidCreateDto()
        {
            return new ReportCreateDto
            {
                CommentId = COMMENT_ID,
                ReportMotiveId = MOTIVE_SPAM_ID,
                Description = "New Spam Report"
            };
        }

        protected override ReportUpdateDto GetValidUpdateDto(Report entityToUpdate)
        {
            return new ReportUpdateDto
            {
                Id = entityToUpdate.Id,
                ReportStatusId = STATUS_RESOLVED_ID
            };
        }

        protected override void SetIdInUpdateDto(ReportUpdateDto dto, int id)
        {
            dto.Id = id;
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");
            await base.Post_ValidObject_ShouldCreateAndReturn200();
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            await Post_ValidObject_ShouldCreateAndReturn200();
        }

        [TestMethod]
        public async Task Put_Admin_ShouldSuccess_AndSetAdminId()
        {
            // Given
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var updateDto = GetValidUpdateDto(new Report { Id = _reportId1 });

            // When
            var result = await _controller.Put(_reportId1, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updated = await _context.Reports.FindAsync(_reportId1);

            Assert.AreEqual(STATUS_RESOLVED_ID, updated.ReportStatusId);
            Assert.AreEqual(USER_ADMIN_ID, updated.AdminId, "L'ID de l'admin ayant traité le report doit être enregistré.");
        }


        [TestMethod]
        public async Task Search_Admin_ShouldReturnResults()
        {
            // Given
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var filter = new ReportFilterDto
            {
                StatusId = STATUS_PENDING_ID
            };

            // When
            var result = await _controller.Search(filter);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<ReportDto>;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(r => r.Id == _reportId1));
            Assert.IsFalse(list.Any(r => r.Id == _reportId2));
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var result = await _controller.GetById(_reportId1);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as ReportDto;
            Assert.AreEqual(_reportId1, dto.Id);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<ReportDto>;
            Assert.IsTrue(list.Any(r => r.Id == _reportId1));
        }
    }
}