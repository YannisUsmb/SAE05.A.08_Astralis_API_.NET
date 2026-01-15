using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Models.Repository.Specific;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class UserNotificationTypesControllerTestsMock : JoinControllerMockTests<UserNotificationTypesController, UserNotificationType, UserNotificationTypeDto, UserNotificationTypeCreateDto, int, int>
    {
        private Mock<INotificationTypeRepository> _mockNotificationTypeRepository;
        private Mock<IUserNotificationTypeRepository> _mockSpecificRepo;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupUser(1);
        }

        protected override UserNotificationTypesController CreateController(Mock<IJoinRepository<UserNotificationType, int, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockNotificationTypeRepository = new Mock<INotificationTypeRepository>();
            _mockSpecificRepo = mockRepo.As<IUserNotificationTypeRepository>();

            return new UserNotificationTypesController(_mockSpecificRepo.Object, _mockNotificationTypeRepository.Object, mapper);
        }

        private void SetupUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        protected override int GetExistingKey1() => 1;
        protected override int GetExistingKey2() => 10;
        protected override int GetNonExistingKey1() => 99;
        protected override int GetNonExistingKey2() => 99;

        protected override UserNotificationType GetSampleEntity() => new UserNotificationType
        {
            UserId = 1,
            NotificationTypeId = 10,
            ByMail = true
        };

        protected override List<UserNotificationType> GetSampleEntities() => new List<UserNotificationType>
        {
            new UserNotificationType { UserId = 1, NotificationTypeId = 10, ByMail = true },
            new UserNotificationType { UserId = 1, NotificationTypeId = 11, ByMail = false }
        };

        protected override UserNotificationTypeCreateDto GetValidCreateDto() => new UserNotificationTypeCreateDto
        {
            NotificationTypeId = 12,
            ByMail = true
        };

        private UserNotificationTypeUpdateDto GetValidUpdateDto(int notifId, bool byMail) => new UserNotificationTypeUpdateDto
        {
            NotificationTypeId = notifId,
            ByMail = byMail
        };

        [TestMethod]
        public async Task GetById_CompositeKey_ShouldReturnDto()
        {
            // Given
            int userId = GetExistingKey1();
            int notifId = GetExistingKey2();
            var entity = GetSampleEntity();

            _mockRepository.Setup(r => r.GetByIdAsync(userId, notifId)).ReturnsAsync(entity);

            // When
            var result = await _controller.GetById(userId, notifId);

            // Then
            var actionResult = result.Result as OkObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Value, typeof(UserNotificationTypeDto));
            _mockRepository.Verify(r => r.GetByIdAsync(userId, notifId), Times.Once);
        }

        [TestMethod]
        public async Task Put_UpdateExisting_ShouldCallUpdate()
        {
            // Given
            int userId = 1;
            int notifId = 10;
            SetupUser(userId);

            var updateDto = GetValidUpdateDto(notifId, false);
            var existingEntity = GetSampleEntity();

            _mockRepository.Setup(r => r.GetByIdAsync(userId, notifId)).ReturnsAsync(existingEntity);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserNotificationType>(), It.IsAny<UserNotificationType>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(userId, notifId, updateDto);

            // Then
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<UserNotificationType>(), It.IsAny<UserNotificationType>()), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Put_CreateNew_ShouldCallAdd()
        {
            // Given
            int userId = 1;
            int notifId = 20;
            SetupUser(userId);

            var updateDto = GetValidUpdateDto(notifId, true);

            _mockRepository.SetupSequence(r => r.GetByIdAsync(userId, notifId))
                .ReturnsAsync((UserNotificationType?)null)
                .ReturnsAsync(new UserNotificationType { UserId = userId, NotificationTypeId = notifId });

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserNotificationType>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(userId, notifId, updateDto);

            // Then
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserNotificationType>()), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Put_IdMismatch_ShouldReturnBadRequest()
        {
            // Given
            int userId = 1;
            int urlNotifId = 10;
            int dtoNotifId = 999;
            var updateDto = GetValidUpdateDto(dtoNotifId, true);

            // When
            var result = await _controller.Put(userId, urlNotifId, updateDto);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Put_WrongUser_ShouldReturnForbid()
        {
            // Given
            int tokenUserId = 1;
            int targetUserId = 2;
            SetupUser(tokenUserId);

            var updateDto = GetValidUpdateDto(10, true);

            // When
            var result = await _controller.Put(targetUserId, 10, updateDto);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WithListOfDtos()
        {
            // Given
            int userId = 1;
            var entities = GetSampleEntities();

            _mockSpecificRepo.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(entities);

            // When
            var result = await _controller.GetAll();

            // Then
            _mockSpecificRepo.Verify(r => r.GetByUserIdAsync(userId), Times.Once);

            _mockRepository.Verify(r => r.GetAllAsync(), Times.Never);

            var actionResult = result.Result as OkObjectResult;
            Assert.IsNotNull(actionResult);
            var returnedDtos = actionResult.Value as IEnumerable<UserNotificationTypeDto>;
            Assert.IsNotNull(returnedDtos);
        }
    }
}