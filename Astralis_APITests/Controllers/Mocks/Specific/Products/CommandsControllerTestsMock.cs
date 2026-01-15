using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class CommandsControllerTestsMock : CrudControllerMockTests<CommandsController, Command, CommandListDto, CommandDetailDto, CommandCheckoutDto, CommandUpdateDto, int>
    {
        private Mock<ICommandRepository> _mockCommandRepository;
        private Mock<ICartItemRepository> _mockCartItemRepository;
        private Mock<IOrderDetailRepository> _mockOrderDetailRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IConfiguration> _mockConfiguration;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Admin");
        }

        protected override CommandsController CreateController(Mock<IReadableRepository<Command, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockCommandRepository = new Mock<ICommandRepository>();
            _mockCartItemRepository = new Mock<ICartItemRepository>();
            _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockCrudRepository = _mockCommandRepository.As<ICrudRepository<Command, int>>();
            _mockRepository = _mockCommandRepository.As<IReadableRepository<Command, int>>();

            _mockCartItemRepository.Setup(r => r.GetByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        ProductNavigation = new Product { Price = 100, Label = "Test Product" }
                    }
                });

            return new CommandsController(
                _mockCommandRepository.Object,
                _mockCartItemRepository.Object,
                _mockOrderDetailRepository.Object,
                mapper,
                _mockConfiguration.Object,
                _mockUserRepository.Object,
                _mockEmailService.Object
            );
        }

        protected override void SetIdInUpdateDto(CommandUpdateDto dto, int id) { }

        protected override List<Command> GetSampleEntities() => new List<Command>
        {
            new Command
            {
                Id = 1,
                UserId = 1,
                CommandStatusId = 1,
                Date = new DateTime(2025, 12, 31),
                Total = 100,
                CommandStatusNavigation = new CommandStatus { Label = "Pending" },
                OrderDetails = new List<OrderDetail>()
            },
            new Command
            {
                Id = 2,
                UserId = 2,
                CommandStatusId = 2,
                Date = new DateTime(2023, 01, 01),
                Total = 200,
                CommandStatusNavigation = new CommandStatus { Label = "Validated" },
                OrderDetails = new List<OrderDetail>()
            }
        };

        protected override Command GetSampleEntity() => new Command
        {
            Id = 1,
            UserId = 1,
            CommandStatusId = 1,
            Date = new DateTime(2025, 12, 31),
            Total = 100,
            CommandStatusNavigation = new CommandStatus { Label = "Pending" },
            OrderDetails = new List<OrderDetail>()
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override CommandCheckoutDto GetValidCreateDto() => new CommandCheckoutDto
        {
            DeliveryAddressId = 1,
            InvoicingAddressId = 1,
            PaymentToken = "tok_test"
        };

        protected override CommandUpdateDto GetValidUpdateDto() => new CommandUpdateDto
        {
            CommandStatusId = 2
        };

        private void SetupHttpContext(int userId, string role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            if (_controller != null)
                _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        }

        [TestMethod]
        public new async Task GetById_ExistingId_ShouldReturnOk_WithDto()
        {
            // Given
            int id = GetExistingId();
            Command entity = GetSampleEntity();

            _mockRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(entity);

            // When
            ActionResult<CommandDetailDto> actionResult = await _controller.GetById(id);

            // Then
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);

            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            CommandDetailDto actualDto = okResult.Value as CommandDetailDto;

            Assert.IsNotNull(actualDto);
            Assert.AreEqual(entity.Id, actualDto.Id);
            Assert.AreEqual(entity.Total, actualDto.Total);
            Assert.AreEqual(entity.CommandStatusNavigation.Label, actualDto.CommandStatusLabel);
        }


        [TestMethod]
        public async Task GetAll_AsUser_ShouldReturnOnlyOwnCommands()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId, "User");
            var entities = GetSampleEntities();
            _mockCommandRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // When
            var result = await _controller.GetAll();

            // Then
            var okResult = result.Result as OkObjectResult;
            var returnedDtos = okResult?.Value as IEnumerable<CommandListDto>;
            Assert.IsNotNull(returnedDtos);
            Assert.AreEqual(1, returnedDtos.Count());
            Assert.IsTrue(returnedDtos.All(c => c.Total == 100));
        }

        [TestMethod]
        public async Task GetById_NotOwner_ShouldReturnForbid()
        {
            // Given
            int ownerId = 1;
            int attackerId = 2;
            int commandId = 1;
            SetupHttpContext(attackerId, "User");

            var entity = GetSampleEntity();
            entity.UserId = ownerId;

            _mockCommandRepository.Setup(r => r.GetByIdAsync(commandId)).ReturnsAsync(entity);

            // When
            var result = await _controller.GetById(commandId);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Post_WithEmptyCart_ShouldReturnBadRequest()
        {
            SetupHttpContext(1, "User");
            _mockCartItemRepository.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<CartItem>());
            var dto = GetValidCreateDto();

            var result = await _controller.Post(dto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            _mockCommandRepository.Verify(r => r.AddAsync(It.IsAny<Command>()), Times.Never);
        }

        [TestMethod]
        public async Task Post_WithValidCart_ShouldCreateCommandAndDetails()
        {
            int userId = 1;
            SetupHttpContext(userId, "User");

            var cartItems = new List<CartItem>
            {
                new CartItem
                {
                    ProductId = 10,
                    Quantity = 2,
                    ProductNavigation = new Product { Price = 50 }
                }
            };

            _mockCartItemRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(cartItems);
            _mockCommandRepository.Setup(r => r.AddAsync(It.IsAny<Command>())).Returns(Task.CompletedTask);
            _mockOrderDetailRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<OrderDetail>>())).Returns(Task.CompletedTask);
            _mockCartItemRepository.Setup(r => r.ClearCartAsync(userId)).Returns(Task.CompletedTask);

            var dto = GetValidCreateDto();

            var result = await _controller.Post(dto);

            _mockCommandRepository.Verify(r => r.AddAsync(It.Is<Command>(c => c.UserId == userId && c.Total == 100)), Times.Once);
            _mockOrderDetailRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<OrderDetail>>(l => l.Count() == 1)), Times.Once);
            _mockCartItemRepository.Verify(r => r.ClearCartAsync(userId), Times.Once);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Put_AsAdmin_ShouldCallUpdate()
        {
            SetupHttpContext(1, "Admin");
            int id = GetExistingId();
            var updateDto = GetValidUpdateDto();
            var entity = GetSampleEntity();

            _mockCommandRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockCommandRepository.Setup(r => r.UpdateAsync(It.IsAny<Command>(), It.IsAny<Command>())).Returns(Task.CompletedTask);

            var result = await _controller.Put(id, updateDto);

            _mockCommandRepository.Verify(r => r.UpdateAsync(It.IsAny<Command>(), It.IsAny<Command>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_AsAdmin_ShouldCallDelete()
        {
            SetupHttpContext(1, "Admin");
            int id = GetExistingId();
            var entity = GetSampleEntity();

            _mockCommandRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockCommandRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            _mockCommandRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}