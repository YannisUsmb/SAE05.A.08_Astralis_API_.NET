using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Models.Repository.Specific;
using Astralis_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class UserNotificationsControllerTestsMock : CrudControllerMockTests<UserNotificationsController, UserNotification, UserNotificationDto, UserNotificationDto, UserNotificationCreateDto, UserNotificationUpdateDto, int>
    {
        private Mock<IUserNotificationRepository> _mockUserNotificationRepository;
        private Mock<INotificationRepository> _mockNotificationRepository;
        private Mock<IUserNotificationTypeRepository> _mockUserNotificationTypeRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IEmailService> _mockEmailService;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupUser(1);
        }

        protected override UserNotificationsController CreateController(Mock<IReadableRepository<UserNotification, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockUserNotificationRepository = new Mock<IUserNotificationRepository>();
            _mockNotificationRepository = new Mock<INotificationRepository>();
            _mockUserNotificationTypeRepository = new Mock<IUserNotificationTypeRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockEmailService = new Mock<IEmailService>();

            _mockRepository = _mockUserNotificationRepository.As<IReadableRepository<UserNotification, int>>();
            _mockCrudRepository = _mockUserNotificationRepository.As<ICrudRepository<UserNotification, int>>();

            return new UserNotificationsController(
                _mockUserNotificationRepository.Object,
                _mockNotificationRepository.Object,
                _mockUserNotificationTypeRepository.Object,
                _mockUserRepository.Object,
                _mockEmailService.Object,
                mapper
            );
        }

        private void SetupUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }


        protected override void SetIdInUpdateDto(UserNotificationUpdateDto dto, int id)
        {
        }

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override UserNotification GetSampleEntity() => new UserNotification
        {
            Id = 1,
            UserId = 1,
            NotificationId = 10,
            IsRead = false,
            ReceivedAt = DateTime.Now
        };

        protected override List<UserNotification> GetSampleEntities() => new List<UserNotification>
        {
            new UserNotification { Id = 1, UserId = 1, NotificationId = 10, IsRead = false },
            new UserNotification { Id = 2, UserId = 1, NotificationId = 11, IsRead = true }
        };

        protected override UserNotificationCreateDto GetValidCreateDto() => new UserNotificationCreateDto
        {
            UserId = 1,
            NotificationId = 10,
            IsRead = false,
            ReceivedAt = DateTime.Now
        };

        protected override UserNotificationUpdateDto GetValidUpdateDto() => new UserNotificationUpdateDto
        {
            UserId = 1,
            NotificationId = 10,
            IsRead = true
        };

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnCreated()
        {
            // Given
            var createDto = GetValidCreateDto();
            int notificationTypeId = 5;

            var notification = new Notification { Id = createDto.NotificationId, NotificationTypeId = notificationTypeId, Label = "Test Notif" };
            _mockNotificationRepository.Setup(r => r.GetByIdAsync(createDto.NotificationId)).ReturnsAsync(notification);

            var user = new User { Id = createDto.UserId, Email = "test@user.com" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(createDto.UserId)).ReturnsAsync(user);

            var userPref = new UserNotificationType { UserId = createDto.UserId, NotificationTypeId = notificationTypeId, ByMail = true };
            _mockUserNotificationTypeRepository.Setup(r => r.GetByIdAsync(createDto.UserId, notificationTypeId)).ReturnsAsync(userPref);

            _mockEmailService.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _mockUserNotificationRepository.Setup(r => r.AddAsync(It.IsAny<UserNotification>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockUserNotificationRepository.Verify(r => r.AddAsync(It.IsAny<UserNotification>()), Times.Once);

            _mockEmailService.Verify(s => s.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            var updateDto = GetValidUpdateDto();
            int id = GetExistingId();

            var existingEntity = GetSampleEntity();
            _mockUserNotificationRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntity);

            _mockUserNotificationRepository.Setup(r => r.UpdateAsync(It.IsAny<UserNotification>(), It.IsAny<UserNotification>()))
                .Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            _mockUserNotificationRepository.Verify(r => r.UpdateAsync(It.IsAny<UserNotification>(), It.IsAny<UserNotification>()), Times.Once);
        }

        [TestMethod]
        public async Task Post_WithByMailFalse_ShouldNotSendEmail()
        {
            // Given
            var createDto = GetValidCreateDto();
            int notificationTypeId = 5;

            _mockNotificationRepository.Setup(r => r.GetByIdAsync(createDto.NotificationId))
                .ReturnsAsync(new Notification { Id = createDto.NotificationId, NotificationTypeId = notificationTypeId });

            _mockUserRepository.Setup(r => r.GetByIdAsync(createDto.UserId))
                .ReturnsAsync(new User { Id = createDto.UserId, Email = "test@user.com" });

            _mockUserNotificationTypeRepository.Setup(r => r.GetByIdAsync(createDto.UserId, notificationTypeId))
                .ReturnsAsync(new UserNotificationType { UserId = createDto.UserId, NotificationTypeId = notificationTypeId, ByMail = false });

            _mockUserNotificationRepository.Setup(r => r.AddAsync(It.IsAny<UserNotification>())).Returns(Task.CompletedTask);

            // When
            await _controller.Post(createDto);

            // Then
            _mockUserNotificationRepository.Verify(r => r.AddAsync(It.IsAny<UserNotification>()), Times.Once);

            _mockEmailService.Verify(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}