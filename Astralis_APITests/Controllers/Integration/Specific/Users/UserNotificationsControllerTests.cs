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
    public class UserNotificationsControllerTests
        : JoinControllerTests<UserNotificationsController, UserNotification, UserNotificationDto, UserNotificationCreateDto, int, int>
    {
        private const int USER_ID = 5002;
        private const int OTHER_USER_ID = 5003;
        private const int NOTIF_ID_LINKED = 88001;
        private const int NOTIF_ID_UNLINKED = 88002;

        protected override UserNotificationsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var repository = new UserNotificationManager(context);
            var controller = new UserNotificationsController(repository, mapper);

            // Simuler l'utilisateur connecté (USER_ID)
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, USER_ID.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            return controller;
        }

        protected override List<UserNotification> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // Seed dependencies
            if (!_context.Users.Any(u => u.Id == USER_ID))
                _context.Users.Add(new User { Id = USER_ID, LastName = "A", FirstName = "B", Email = "a@b.com", Username = "UserA", UserRoleId = 1, AvatarUrl = "url", Password = "pwd" });

            if (!_context.Users.Any(u => u.Id == OTHER_USER_ID))
                _context.Users.Add(new User { Id = OTHER_USER_ID, LastName = "X", FirstName = "Y", Email = "x@y.com", Username = "UserX", UserRoleId = 1, AvatarUrl = "url", Password = "pwd" });

            if (!_context.NotificationTypes.Any())
                _context.NotificationTypes.Add(new NotificationType { Id = 1, Label = "Info" });

            if (!_context.Notifications.Any(n => n.Id == NOTIF_ID_LINKED))
                _context.Notifications.Add(new Notification { Id = NOTIF_ID_LINKED, Label = "N1", NotificationTypeId = 1 });

            if (!_context.Notifications.Any(n => n.Id == NOTIF_ID_UNLINKED))
                _context.Notifications.Add(new Notification { Id = NOTIF_ID_UNLINKED, Label = "N2", NotificationTypeId = 1 });

            _context.SaveChanges();

            // Seed Join Entity
            return new List<UserNotification>
            {
                new UserNotification
                {
                    UserId = USER_ID,
                    NotificationId = NOTIF_ID_LINKED,
                    IsRead = false,
                    ReceivedAt = DateTime.UtcNow
                }
            };
        }

        // --- Abstract implementations ---

        protected override int GetKey1(UserNotification entity) => entity.UserId;
        protected override int GetKey2(UserNotification entity) => entity.NotificationId;
        protected override int GetNonExistingKey1() => 99999;
        protected override int GetNonExistingKey2() => 99999;

        protected override UserNotificationCreateDto GetValidCreateDto()
        {
            return new UserNotificationCreateDto
            {
                UserId = USER_ID,
                NotificationId = NOTIF_ID_UNLINKED,
                IsRead = false,
                ReceivedAt = DateTime.UtcNow
            };
        }

        protected override (int, int) GetKeysFromCreateDto(UserNotificationCreateDto dto)
        {
            return (dto.UserId, dto.NotificationId);
        }

        // --- Custom Tests ---

        [TestMethod]
        public async Task Put_MarkAsRead_ShouldSuccess()
        {
            // Given
            var updateDto = new UserNotificationUpdateDto
            {
                UserId = USER_ID,
                NotificationId = NOTIF_ID_LINKED,
                IsRead = true
            };

            // When
            var result = await _controller.Put(USER_ID, NOTIF_ID_LINKED, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var entity = await _context.UserNotifications.FindAsync(USER_ID, NOTIF_ID_LINKED);
            Assert.IsTrue(entity!.IsRead);
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldReturnForbidden()
        {
            // Given
            var updateDto = new UserNotificationUpdateDto
            {
                UserId = OTHER_USER_ID,
                NotificationId = NOTIF_ID_LINKED,
                IsRead = true
            };

            // When
            var result = await _controller.Put(OTHER_USER_ID, NOTIF_ID_LINKED, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_IdsMismatch_ShouldReturnBadRequest()
        {
            // Given
            var updateDto = new UserNotificationUpdateDto
            {
                UserId = USER_ID,
                NotificationId = NOTIF_ID_UNLINKED, // Body mismatch
                IsRead = true
            };

            // When
            var result = await _controller.Put(USER_ID, NOTIF_ID_LINKED, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}