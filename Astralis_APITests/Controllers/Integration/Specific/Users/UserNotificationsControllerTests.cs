using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    public class FakeEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string htmlMessage) => Task.CompletedTask;
    }

    [TestClass]
    public class UserNotificationsControllerTests
        : CrudControllerTests<UserNotificationsController, UserNotification, UserNotificationDto, UserNotificationDto, UserNotificationCreateDto, UserNotificationUpdateDto, int>
    {
        private const int USER_ID = 5002;
        private const int OTHER_USER_ID = 5003;
        private const int NOTIF_ID_LINKED = 88001;
        private const int NOTIF_ID_UNLINKED = 88002;
        private const int USER_NOTIF_ID = 100;

        protected override UserNotificationsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var userNotifRepo = new UserNotificationManager(context);
            var notifRepo = new NotificationManager(context);
            var typeRepo = new UserNotificationTypeManager(context);
            var userRepo = new UserManager(context);
            var emailService = new FakeEmailService();

            var controller = new UserNotificationsController(
                userNotifRepo, notifRepo, typeRepo, userRepo, emailService, mapper
            );

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

            return new List<UserNotification>
            {
                new UserNotification
                {
                    Id = USER_NOTIF_ID,
                    UserId = USER_ID,
                    NotificationId = NOTIF_ID_LINKED,
                    IsRead = false,
                    ReceivedAt = DateTime.UtcNow
                }
            };
        }
        protected override int GetIdFromEntity(UserNotification entity) => entity.Id;

        protected override int GetNonExistingId() => 99999;

        protected override int GetIdFromDto(UserNotificationDto dto) => dto.Id;

        protected override UserNotificationUpdateDto GetValidUpdateDto(UserNotification entity)
        {
            return new UserNotificationUpdateDto
            {
                UserId = entity.UserId,
                NotificationId = entity.NotificationId,
                IsRead = !entity.IsRead 
            };
        }

        protected override void SetIdInUpdateDto(UserNotificationUpdateDto dto, int id)
        {
        }


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

        [TestMethod]
        public async Task Put_MarkAsRead_ReturnsOk()
        {
            // Given
            var updateDto = new UserNotificationUpdateDto
            {
                UserId = USER_ID,
                NotificationId = NOTIF_ID_LINKED,
                IsRead = true
            };

            // When
            var result = await _controller.Put(USER_NOTIF_ID, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var entity = await _context.UserNotifications.FindAsync(USER_NOTIF_ID);
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

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, OTHER_USER_ID.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            // When
            var result = await _controller.Put(USER_NOTIF_ID, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public new async Task Put_ValidObject_ShouldUpdateAndReturn204()
        {
            // Arrange
            var entity = _seededEntities.First();
            var id = GetIdFromEntity(entity);
            var updateDto = GetValidUpdateDto(entity);

            // Act
            var actionResult = await _controller.Put(id, updateDto);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
        }
    }
}