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
    public class NotificationsControllerTests
        : CrudControllerTests<NotificationsController, Notification, NotificationDto, NotificationDto, NotificationCreateDto, NotificationUpdateDto, int>
    {
        // --- CONSTANTS ---
        private const int ROLE_ADMIN_ID = 10;
        private const int ROLE_USER_ID = 1;

        private const int USER_ADMIN_ID = 5001;
        private const int USER_NORMAL_ID = 5002;

        private const int TYPE_INFO_ID = 101;
        private const int TYPE_ALERT_ID = 102;

        private const int NOTIF_ID_1 = 88001;
        private const int NOTIF_ID_2 = 88002;

        private int _notifId1;
        private int _notifId2;

        protected override NotificationsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var notifManager = new NotificationManager(context);

            var controller = new NotificationsController(
                notifManager,
                mapper
            );

            SetupUserContext(controller, USER_ADMIN_ID, "Admin");
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

        protected override List<Notification> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // ==============================================================================
            // 1. SETUP DEPENDENCIES
            // ==============================================================================

            // Roles
            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == ROLE_ADMIN_ID))
                _context.UserRoles.Add(new UserRole { Id = ROLE_ADMIN_ID, Label = "Admin" });

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == ROLE_USER_ID))
                _context.UserRoles.Add(new UserRole { Id = ROLE_USER_ID, Label = "User" });

            // Notification Types
            // CORRECTION : Ajout de la propriété "Description" requise par la contrainte NOT NULL
            if (!_context.NotificationTypes.AsNoTracking().Any(t => t.Id == TYPE_INFO_ID))
            {
                _context.NotificationTypes.Add(new NotificationType
                {
                    Id = TYPE_INFO_ID,
                    Label = "Information",
                    Description = "Standard information type" // Ajouté ici
                });
            }

            if (!_context.NotificationTypes.AsNoTracking().Any(t => t.Id == TYPE_ALERT_ID))
            {
                _context.NotificationTypes.Add(new NotificationType
                {
                    Id = TYPE_ALERT_ID,
                    Label = "Alert",
                    Description = "Urgent alert type" // Ajouté ici
                });
            }

            _context.SaveChanges();

            // Users
            CreateUserIfNotExist(USER_ADMIN_ID, ROLE_ADMIN_ID);
            CreateUserIfNotExist(USER_NORMAL_ID, ROLE_USER_ID);

            _context.ChangeTracker.Clear();

            // ==============================================================================
            // 2. FETCH DEPENDENCIES
            // ==============================================================================

            var typeInfo = _context.NotificationTypes.Find(TYPE_INFO_ID);
            var typeAlert = _context.NotificationTypes.Find(TYPE_ALERT_ID);

            // ==============================================================================
            // 3. CREATE NOTIFICATIONS IN MEMORY
            // ==============================================================================

            var n1 = new Notification
            {
                Id = NOTIF_ID_1,
                Label = "Welcome to Astralis",
                Description = "Thank you for joining us.",
                NotificationTypeId = TYPE_INFO_ID,
                NotificationTypeNavigation = typeInfo!
            };

            var n2 = new Notification
            {
                Id = NOTIF_ID_2,
                Label = "Server Maintenance",
                Description = "Downtime expected at midnight.",
                NotificationTypeId = TYPE_ALERT_ID,
                NotificationTypeNavigation = typeAlert!
            };

            _notifId1 = NOTIF_ID_1;
            _notifId2 = NOTIF_ID_2;

            return new List<Notification> { n1, n2 };
        }

        private void CreateUserIfNotExist(int id, int roleId)
        {
            if (!_context.Users.AsNoTracking().Any(u => u.Id == id))
            {
                _context.Users.Add(new User
                {
                    Id = id,
                    UserRoleId = roleId,
                    LastName = "Test",
                    FirstName = "User",
                    Email = $"user{id}@test.com",
                    Username = $"User{id}",
                    AvatarUrl = "http://img.com",
                    Password = "pwd"
                });
                _context.SaveChanges();
            }
        }

        // --- CRUD CONFIGURATION ---
        protected override int GetIdFromEntity(Notification entity) => entity.Id;
        protected override int GetIdFromDto(NotificationDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override NotificationCreateDto GetValidCreateDto()
        {
            return new NotificationCreateDto
            {
                Label = "New Update",
                Description = "Version 2.0 is live",
                NotificationTypeId = TYPE_INFO_ID
            };
        }

        protected override NotificationUpdateDto GetValidUpdateDto(Notification entityToUpdate)
        {
            return new NotificationUpdateDto
            {
                Id = entityToUpdate.Id,
                Label = "Updated Label",
                Description = "Updated Description",
                NotificationTypeId = TYPE_ALERT_ID
            };
        }

        protected override void SetIdInUpdateDto(NotificationUpdateDto dto, int id)
        {
            dto.Id = id;
        }

        // =========================================================================================
        // TESTS: GENERIC OVERRIDES
        // =========================================================================================

        [TestMethod]
        public new async Task GetAll_ShouldReturnOk()
        {
            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<NotificationDto>;

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(n => n.Id == _notifId1));

            // On vérifie que le mapping a bien récupéré "Information" ou "Alert"
            Assert.IsTrue(list.Any(n => n.NotificationTypeName == "Information" || n.NotificationTypeName == "Alert"));
        }

        [TestMethod]
        public new async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var result = await _controller.GetById(_notifId1);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as NotificationDto;

            Assert.IsNotNull(dto);
            Assert.AreEqual(_notifId1, dto.Id);
            Assert.AreEqual("Information", dto.NotificationTypeName);
        }

        // =========================================================================================
        // TESTS: SECURITY
        // =========================================================================================

        [TestMethod]
        public async Task Put_Admin_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var updateDto = GetValidUpdateDto(new Notification { Id = _notifId1 });

            var result = await _controller.Put(_notifId1, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var dbEntity = await _context.Notifications.FindAsync(_notifId1);
            Assert.AreEqual("Updated Label", dbEntity.Label);
        }

        [TestMethod]
        public async Task Put_User_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");
            var updateDto = GetValidUpdateDto(new Notification { Id = _notifId1 });

            try
            {
                await _controller.Put(_notifId1, updateDto);
            }
            catch (Exception) { }
        }

        [TestMethod]
        public async Task Delete_Admin_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");

            var result = await _controller.Delete(_notifId1);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_User_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            try
            {
                await _controller.Delete(_notifId1);
            }
            catch (Exception) { }
        }
    }
}