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
    public class UserNotificationTypesControllerTests
        : JoinControllerTests<UserNotificationTypesController, UserNotificationType, UserNotificationTypeDto, UserNotificationTypeCreateDto, int, int>
    {
        private const int ROLE_USER_ID = 1;
        private const int USER_NORMAL_ID = 5002;
        private const int USER_OTHER_ID = 5003;

        private const int TYPE_INFO_ID = 101;
        private const int TYPE_ALERT_ID = 102;

        protected override UserNotificationTypesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var repository = new UserNotificationTypeManager(context);

            var controller = new UserNotificationTypesController(
                repository,
                mapper
            );

            SetupUserContext(controller, USER_NORMAL_ID);
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, $"User_{userId}")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<UserNotificationType> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == ROLE_USER_ID))
                _context.UserRoles.Add(new UserRole { Id = ROLE_USER_ID, Label = "User" });

            if (!_context.NotificationTypes.AsNoTracking().Any(t => t.Id == TYPE_INFO_ID))
                _context.NotificationTypes.Add(new NotificationType { Id = TYPE_INFO_ID, Label = "Information", Description = "Info desc" });

            if (!_context.NotificationTypes.AsNoTracking().Any(t => t.Id == TYPE_ALERT_ID))
                _context.NotificationTypes.Add(new NotificationType { Id = TYPE_ALERT_ID, Label = "Alert", Description = "Alert desc" });

            _context.SaveChanges();

            CreateUserIfNotExist(USER_NORMAL_ID);
            CreateUserIfNotExist(USER_OTHER_ID);

            _context.ChangeTracker.Clear();

            var user = _context.Users.Find(USER_NORMAL_ID);
            var typeInfo = _context.NotificationTypes.Find(TYPE_INFO_ID);

            var joinEntity = new UserNotificationType
            {
                UserId = USER_NORMAL_ID,
                NotificationTypeId = TYPE_INFO_ID,
                ByMail = true,

                UserNavigation = user!,
                NotificationTypeNavigation = typeInfo!
            };

            return new List<UserNotificationType> { joinEntity };
        }

        private void CreateUserIfNotExist(int id)
        {
            if (!_context.Users.AsNoTracking().Any(u => u.Id == id))
            {
                _context.Users.Add(new User
                {
                    Id = id,
                    UserRoleId = ROLE_USER_ID,
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
        protected override int GetKey1(UserNotificationType entity) => entity.UserId;
        protected override int GetKey2(UserNotificationType entity) => entity.NotificationTypeId;

        protected override int GetNonExistingKey1() => 99999;
        protected override int GetNonExistingKey2() => 99999;

        protected override UserNotificationTypeCreateDto GetValidCreateDto()
        {
            
            return new UserNotificationTypeCreateDto
            {
                NotificationTypeId = TYPE_ALERT_ID,
                ByMail = false
            };
        }

        protected override (int, int) GetKeysFromCreateDto(UserNotificationTypeCreateDto dto)
        {
            return (USER_NORMAL_ID, dto.NotificationTypeId);
        }

        [TestMethod]
        public async Task Put_Self_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_NORMAL_ID);

            var updateDto = new UserNotificationTypeUpdateDto
            {
                NotificationTypeId = TYPE_INFO_ID,
                ByMail = false
            };
            var result = await _controller.Put(USER_NORMAL_ID, TYPE_INFO_ID, updateDto);                       
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var dbEntity = await _context.UserNotificationTypes.FindAsync(USER_NORMAL_ID, TYPE_INFO_ID);
            Assert.IsFalse(dbEntity.ByMail);
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID);

            var updateDto = new UserNotificationTypeUpdateDto
            {
                NotificationTypeId = TYPE_INFO_ID,
                ByMail = false
            };

            var result = await _controller.Put(USER_OTHER_ID, TYPE_INFO_ID, updateDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_MismatchIds_ShouldReturnBadRequest()
        {
            SetupUserContext(_controller, USER_NORMAL_ID);

            var updateDto = new UserNotificationTypeUpdateDto
            {
                NotificationTypeId = TYPE_ALERT_ID,
                ByMail = false
            };

            var result = await _controller.Put(USER_NORMAL_ID, TYPE_INFO_ID, updateDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_AndIncludeSeededItems()
        {
            await base.GetAll_ShouldReturnOk_AndIncludeSeededItems();
        }
    }
}