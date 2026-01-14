using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
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
        private const int USER_NORMAL_ID = 5002;
        private const int USER_OTHER_ID = 5003;

        private const int TYPE_INFO_ID = 101;
        private const int TYPE_ALERT_ID = 102;

        protected override UserNotificationTypesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var repository = new UserNotificationTypeManager(context);
            var notifTypeRepository = new NotificationTypeManager(context);

            var controller = new UserNotificationTypesController(
                repository,
                notifTypeRepository,
                mapper
            );

            SetupUserContext(controller, USER_NORMAL_ID);
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        protected override List<UserNotificationType> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            var existingJoin = _context.UserNotificationTypes.Find(USER_NORMAL_ID, TYPE_INFO_ID);
            if (existingJoin != null)
            {
                _context.UserNotificationTypes.Remove(existingJoin);
                _context.SaveChanges();
            }
            _context.ChangeTracker.Clear();
            if (!_context.UserRoles.Any(r => r.Id == 1))
                _context.UserRoles.Add(new UserRole { Id = 1, Label = "User" });

            _context.SaveChanges();

            var user = _context.Users.Find(USER_NORMAL_ID);
            if (user == null)
            {
                user = new User
                {
                    Id = USER_NORMAL_ID,
                    LastName = "N",
                    FirstName = "N",
                    Email = "n@n.com",
                    Username = "UserN",
                    UserRoleId = 1,
                    Password = "pwd"
                };
                _context.Users.Add(user);
            }

            if (!_context.Users.Any(u => u.Id == USER_OTHER_ID))
            {
                _context.Users.Add(new User
                {
                    Id = USER_OTHER_ID,
                    LastName = "O",
                    FirstName = "O",
                    Email = "o@o.com",
                    Username = "Other",
                    UserRoleId = 1,
                    Password = "pwd"
                });
            }

            var typeInfo = _context.NotificationTypes.Find(TYPE_INFO_ID);
            if (typeInfo == null)
            {
                typeInfo = new NotificationType
                {
                    Id = TYPE_INFO_ID,
                    Label = "Info",
                    Description = "Description obligatoire"
                };
                _context.NotificationTypes.Add(typeInfo);
            }

            if (!_context.NotificationTypes.Any(t => t.Id == TYPE_ALERT_ID))
            {
                _context.NotificationTypes.Add(new NotificationType
                {
                    Id = TYPE_ALERT_ID,
                    Label = "Alert",
                    Description = "Description obligatoire"
                });
            }

            _context.SaveChanges();

            var joinEntity = new UserNotificationType
            {
                UserId = USER_NORMAL_ID,
                NotificationTypeId = TYPE_INFO_ID,
                ByMail = true,
                UserNavigation = user,
                NotificationTypeNavigation = typeInfo
            };

            return new List<UserNotificationType> { joinEntity };
        }


        protected override int GetKey1(UserNotificationType entity) => entity.UserId;
        protected override int GetKey2(UserNotificationType entity) => entity.NotificationTypeId;

        protected override int GetNonExistingKey1() => 99999;
        protected override int GetNonExistingKey2() => 88888;

        protected override UserNotificationTypeCreateDto GetValidCreateDto()
        {
            return new UserNotificationTypeCreateDto
            {
                NotificationTypeId = TYPE_ALERT_ID,
                ByMail = true
            };
        }

        protected override (int, int) GetKeysFromCreateDto(UserNotificationTypeCreateDto dto)
        {
            return (USER_NORMAL_ID, dto.NotificationTypeId);
        }


        [TestMethod]
        public async Task Put_UpdatePreference_ShouldWork()
        {
            // Given
            SetupUserContext(_controller, USER_NORMAL_ID);

            var updateDto = new UserNotificationTypeUpdateDto
            {
                NotificationTypeId = TYPE_INFO_ID,
                ByMail = false
            };

            // When
            var result = await _controller.Put(USER_NORMAL_ID, TYPE_INFO_ID, updateDto);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var entity = await _context.UserNotificationTypes.FindAsync(USER_NORMAL_ID, TYPE_INFO_ID);
            Assert.IsFalse(entity!.ByMail);
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, USER_NORMAL_ID);

            var updateDto = new UserNotificationTypeUpdateDto
            {
                NotificationTypeId = TYPE_INFO_ID,
                ByMail = false
            };

            // When
            var result = await _controller.Put(USER_OTHER_ID, TYPE_INFO_ID, updateDto);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(ForbidResult));
        }
    }
}