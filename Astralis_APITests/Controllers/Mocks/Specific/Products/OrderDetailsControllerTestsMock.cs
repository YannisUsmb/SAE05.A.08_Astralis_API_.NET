using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class OrderDetailsControllerTestsMock : JoinControllerMockTests<OrderDetailsController, OrderDetail, OrderDetailDto, OrderDetailCreateDto, int, int>
    {
        private Mock<IOrderDetailRepository> _mockOrderDetailRepository;
        private Mock<ICommandRepository> _mockCommandRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupUser(1, "User");
        }

        protected override OrderDetailsController CreateController(Mock<IJoinRepository<OrderDetail, int, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockCommandRepository = new Mock<ICommandRepository>();
            _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();

            _mockRepository = _mockOrderDetailRepository.As<IJoinRepository<OrderDetail, int, int>>();

            return new OrderDetailsController(_mockOrderDetailRepository.Object, _mockCommandRepository.Object, mapper);
        }

        private void SetupUser(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        protected override int GetExistingKey1() => 100;
        protected override int GetExistingKey2() => 50;
        protected override int GetNonExistingKey1() => 999;
        protected override int GetNonExistingKey2() => 999;

        protected override OrderDetail GetSampleEntity()
        {
            return new OrderDetail
            {
                CommandId = 100,
                ProductId = 50,
                Quantity = 2,
                CommandNavigation = new Command
                {
                    Id = 100,
                    UserId = 1,
                    CommandStatusId = 1
                }
            };
        }

        protected override List<OrderDetail> GetSampleEntities() => new List<OrderDetail> { GetSampleEntity() };

        protected override OrderDetailCreateDto GetValidCreateDto() => new OrderDetailCreateDto
        {
            CommandId = 200,
            ProductId = 60,
            Quantity = 5
        };

        [TestMethod]
        public  async Task Delete_ExistingIds_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id1 = GetExistingKey1();
            int id2 = GetExistingKey2();
            var entity = GetSampleEntity();

            _mockRepository.Setup(r => r.GetByIdAsync(id1, id2)).ReturnsAsync(entity);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<OrderDetail>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id1, id2);

            // Then
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<OrderDetail>()), Times.Once);

            _mockRepository.Verify(r => r.GetByIdAsync(id1, id2), Times.AtLeastOnce);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public  async Task Post_ValidObject_ShouldCallAddAndReturnCreated()
        {
            // Given
            var dto = GetValidCreateDto();
            int userId = 1;

            var command = new Command
            {
                Id = dto.CommandId,
                UserId = userId,
                CommandStatusId = 1
            };
            _mockCommandRepository.Setup(r => r.GetByIdAsync(dto.CommandId)).ReturnsAsync(command);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<OrderDetail>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(dto);

            // Then
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<OrderDetail>()), Times.Once);

            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task Post_CommandNotFound_ShouldReturnNotFound()
        {
            // Given
            var dto = GetValidCreateDto();
            _mockCommandRepository.Setup(r => r.GetByIdAsync(dto.CommandId)).ReturnsAsync((Command?)null);

            // When
            var result = await _controller.Post(dto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }


        [TestMethod]
        public async Task GetById_NotOwner_ShouldReturnForbidden()
        {
            // Given
            int commandId = 100;
            int productId = 50;
            int ownerId = 1;
            int otherUserId = 2;
            SetupUser(otherUserId, "User");

            var entity = new OrderDetail
            {
                CommandId = commandId,
                ProductId = productId,
                CommandNavigation = new Command { Id = commandId, UserId = ownerId }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(commandId, productId)).ReturnsAsync(entity);

            // When
            var result = await _controller.GetById(commandId, productId);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Post_CommandLocked_ShouldReturnBadRequest()
        {
            var dto = GetValidCreateDto();
            int userId = 1;
            SetupUser(userId, "User");

            var command = new Command
            {
                Id = dto.CommandId,
                UserId = userId,
                CommandStatusId = 2
            };

            _mockCommandRepository.Setup(r => r.GetByIdAsync(dto.CommandId)).ReturnsAsync(command);

            var result = await _controller.Post(dto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Delete_CommandLocked_ShouldReturnBadRequest()
        {
            int cmdId = 100;
            int prodId = 50;
            int userId = 1;
            SetupUser(userId, "User");

            var entity = new OrderDetail
            {
                CommandId = cmdId,
                ProductId = prodId,
                CommandNavigation = new Command
                {
                    Id = cmdId,
                    UserId = userId,
                    CommandStatusId = 2
                }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(cmdId, prodId)).ReturnsAsync(entity);

            var result = await _controller.Delete(cmdId, prodId);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            var dto = GetValidCreateDto();
            int userId = 1; 
            var command = new Command
            {
                Id = dto.CommandId,
                UserId = userId,
                CommandStatusId = 1
            };

            _mockCommandRepository.Setup(r => r.GetByIdAsync(dto.CommandId)).ReturnsAsync(command);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<OrderDetail>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(dto);

            // Then
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<OrderDetail>()), Times.Once);            
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}