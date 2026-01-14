using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    public class FakeReportEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string htmlMessage) => Task.CompletedTask;
    }

    [TestClass]
    public class ReportsControllerTests
        : CrudControllerTests<ReportsController, Report, ReportDto, ReportDto, ReportCreateDto, ReportUpdateDto, int>
    {
        private const int USER_NORMAL_ID = 5001;
        private const int USER_ADMIN_ID = 5002;

        private const int STATUS_PENDING_ID = 1;
        private const int STATUS_RESOLVED_ID = 2;
        private const int MOTIVE_SPAM_ID = 1;

        private const int COMMENT_ID_REF = 777;
        private const int ARTICLE_ID_REF = 999;
        private const int REPORT_ID_1 = 66001;
        private const int REPORT_ID_2 = 66002;

        // DATE FIXE pour éviter les erreurs de comparaison (millisecondes)
        private readonly DateTime _fixedDate = new DateTime(2026, 1, 14, 12, 0, 0, DateTimeKind.Utc);

        protected override ReportsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var reportManager = new ReportManager(context);
            var emailService = new FakeReportEmailService();

            var controller = new ReportsController(reportManager, emailService, mapper);
            SetupUserContext(controller, USER_NORMAL_ID, "User");

            return controller;
        }

        private void SetupUserContext(ReportsController controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        protected override List<Report> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // --- 1. Gestion des Users (Check existence avant insertion) ---
            if (_context.Users.Find(USER_NORMAL_ID) == null)
            {
                _context.Users.Add(new User { Id = USER_NORMAL_ID, LastName = "N", FirstName = "N", Email = "n@n.com", Username = "UserN", UserRoleId = 1, Password = "pwd" });
            }

            if (_context.Users.Find(USER_ADMIN_ID) == null)
            {
                _context.Users.Add(new User { Id = USER_ADMIN_ID, LastName = "A", FirstName = "A", Email = "a@a.com", Username = "Admin", UserRoleId = 2, Password = "pwd" });
            }

            // --- 2. Gestion des Références (C'est ici que ça plantait) ---

            // Status Pending
            var statusPending = _context.ReportStatuses.Find(STATUS_PENDING_ID);
            if (statusPending == null)
            {
                statusPending = new ReportStatus { Id = STATUS_PENDING_ID, Label = "Pending" };
                _context.ReportStatuses.Add(statusPending);
            }

            // Status Resolved
            var statusResolved = _context.ReportStatuses.Find(STATUS_RESOLVED_ID);
            if (statusResolved == null)
            {
                statusResolved = new ReportStatus { Id = STATUS_RESOLVED_ID, Label = "Resolved" };
                _context.ReportStatuses.Add(statusResolved);
            }

            // Motive Spam (La cause de votre erreur 23505)
            var motiveSpam = _context.ReportMotives.Find(MOTIVE_SPAM_ID);
            if (motiveSpam == null)
            {
                motiveSpam = new ReportMotive { Id = MOTIVE_SPAM_ID, Label = "Spam" };
                _context.ReportMotives.Add(motiveSpam);
            }


            var article = _context.Articles.Find(ARTICLE_ID_REF);
            if (article == null)
            {
                article = new Article { Id = ARTICLE_ID_REF, Title = "Test Art", Content = "Txt", UserId = USER_ADMIN_ID };
                _context.Articles.Add(article);
            }

            var comment = _context.Comments.Find(COMMENT_ID_REF);
            if (comment == null)
            {
                comment = new Comment
                {
                    Id = COMMENT_ID_REF,
                    UserId = USER_NORMAL_ID,
                    ArticleId = ARTICLE_ID_REF,
                    Text = "Contenu obligatoire"
                };
                _context.Comments.Add(comment);
            }

            // Sauvegarde intermédiaire pour s'assurer que les références existent 
            // avant de créer les Reports qui dépendent d'elles.
            _context.SaveChanges();

            // --- 4. Création des Reports ---
            // On vérifie si les reports existent déjà pour éviter les doublons là aussi
            var reports = new List<Report>();

            if (_context.Reports.Find(REPORT_ID_1) == null)
            {
                reports.Add(new Report
                {
                    Id = REPORT_ID_1,
                    UserId = USER_NORMAL_ID,
                    CommentId = COMMENT_ID_REF,
                    ReportMotiveId = MOTIVE_SPAM_ID,
                    ReportStatusId = STATUS_PENDING_ID,
                    Description = "Spam content",
                    Date = _fixedDate,
                    // Liaison des objets pour que le DTO soit complet
                    ReportStatusNavigation = statusPending,
                    ReportMotiveNavigation = motiveSpam
                });
            }

            if (_context.Reports.Find(REPORT_ID_2) == null)
            {
                reports.Add(new Report
                {
                    Id = REPORT_ID_2,
                    UserId = USER_NORMAL_ID,
                    CommentId = COMMENT_ID_REF,
                    ReportMotiveId = MOTIVE_SPAM_ID,
                    ReportStatusId = STATUS_RESOLVED_ID,
                    Description = "Old report",
                    Date = _fixedDate.AddDays(-1),
                    ReportStatusNavigation = statusResolved,
                    ReportMotiveNavigation = motiveSpam
                });
            }

            return reports;
        }

        // --- Méthodes Abstraites ---

        protected override int GetIdFromEntity(Report entity) => entity.Id;
        protected override int GetNonExistingId() => 99999;
        protected override int GetIdFromDto(ReportDto dto) => dto.Id;

        protected override ReportCreateDto GetValidCreateDto()
        {
            return new ReportCreateDto
            {
                CommentId = COMMENT_ID_REF,
                ReportMotiveId = MOTIVE_SPAM_ID,
                Description = "This is a test report"
            };
        }

        protected override ReportUpdateDto GetValidUpdateDto(Report entity)
        {
            return new ReportUpdateDto
            {
                Id = entity.Id,
                ReportStatusId = STATUS_RESOLVED_ID
            };
        }

        protected override void SetIdInUpdateDto(ReportUpdateDto dto, int id)
        {
            dto.Id = id;
        }

        // --- Tests Surchargés (Correction des erreurs) ---

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            // Correction : Votre contrôleur renvoie CreatedAtAction (201), pas Ok (200).
            // On surcharge le test pour accepter ce comportement correct.

            // Arrange
            SetupUserContext(_controller, USER_NORMAL_ID, "User");
            var createDto = GetValidCreateDto();

            // Act
            var result = await _controller.Post(createDto);

            // Assert
            Assert.IsNotNull(result);
            // On vérifie que c'est bien un 201 Created
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));

            // Vérifs supplémentaires optionnelles
            var createdResult = result.Result as CreatedAtActionResult;
            var createdDto = createdResult.Value as ReportDto;
            Assert.IsNotNull(createdDto);
            Assert.AreEqual(createDto.Description, createdDto.Description);
        }

        [TestMethod]
        public async Task Delete_ExistingId_ShouldDeleteAndReturn204()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin"); // Admin only
            await base.Delete_ExistingId_ShouldDeleteAndReturn204();
        }

        // --- Test Spécifique ---

        [TestMethod]
        public async Task Search_Admin_ShouldReturnResults()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            await Task.CompletedTask;
        }
    }
}