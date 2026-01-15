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
    public class CartItemsControllerTestsMock : JoinControllerMockTests<CartItemsController, CartItem, CartItemDto, CartItemCreateDto, int, int>
    {
        private Mock<ICartItemRepository> _mockCartItemRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override CartItemsController CreateController(Mock<IJoinRepository<CartItem, int, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockCartItemRepository = new Mock<ICartItemRepository>();
            _mockRepository = _mockCartItemRepository.As<IJoinRepository<CartItem, int, int>>();

            return new CartItemsController(_mockCartItemRepository.Object, mapper);
        }
        protected override List<CartItem> GetSampleEntities() => new List<CartItem>
        {
            new CartItem
            {
                UserId = 1,
                ProductId = 10,
                Quantity = 2,
                ProductNavigation = new Product { Id = 10, Label = "Produit A", Price = 10, ProductPictureUrl = "img1.jpg" }
            },
            new CartItem
            {
                UserId = 1,
                ProductId = 20,
                Quantity = 1,
                ProductNavigation = new Product { Id = 20, Label = "Produit B", Price = 20, ProductPictureUrl = "img2.jpg" }
            }
        };

        protected override CartItem GetSampleEntity() => new CartItem
        {
            UserId = 1,
            ProductId = 10,
            Quantity = 5,
            ProductNavigation = new Product { Id = 10, Label = "Produit A", Price = 10 }
        };

        protected override int GetExistingKey1() => 1;
        protected override int GetExistingKey2() => 10;

        protected override int GetNonExistingKey1() => 1;
        protected override int GetNonExistingKey2() => 999;

        protected override CartItemCreateDto GetValidCreateDto() => new CartItemCreateDto
        {
            ProductId = 30,
            Quantity = 3
        };

        // --- Helper Context ---
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
        public async Task GetAll_ShouldReturnOk_WithListOfDtos()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId);
            var entities = GetSampleEntities();
            _mockCartItemRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(entities);

            // When
            var result = await _controller.GetAll();

            // Then
            _mockCartItemRepository.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            var dtos = okResult.Value as IEnumerable<CartItemDto>;
            Assert.IsNotNull(dtos);
            Assert.AreEqual(2, dtos.Count());
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId);
            var createDto = GetValidCreateDto();

            _mockCartItemRepository.Setup(r => r.GetByIdAsync(userId, createDto.ProductId)).ReturnsAsync((CartItem)null);
            _mockCartItemRepository.Setup(r => r.AddAsync(It.IsAny<CartItem>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockCartItemRepository.Verify(r => r.AddAsync(It.Is<CartItem>(c => c.UserId == userId && c.ProductId == createDto.ProductId)), Times.Once);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }


        [TestMethod]
        public async Task Post_ExistingItem_ShouldUpdateQuantity()
        {
            // Given
            int userId = 1;
            int productId = 10;
            int initialQty = 5;
            int addedQty = 2;
            SetupHttpContext(userId);

            var createDto = new CartItemCreateDto { ProductId = productId, Quantity = addedQty };
            var existingItem = new CartItem { UserId = userId, ProductId = productId, Quantity = initialQty, ProductNavigation = new Product() };

            _mockCartItemRepository.Setup(r => r.GetByIdAsync(userId, productId)).ReturnsAsync(existingItem);
            _mockCartItemRepository.Setup(r => r.UpdateAsync(existingItem, existingItem)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            Assert.AreEqual(initialQty + addedQty, existingItem.Quantity);
            _mockCartItemRepository.Verify(r => r.UpdateAsync(existingItem, existingItem), Times.Once);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetMyCart_ShouldReturnCartDto()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId);
            var entities = GetSampleEntities();
            _mockCartItemRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(entities);

            // When
            var result = await _controller.GetMyCart();

            // Then
            _mockCartItemRepository.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsTrue(okResult.Value.GetType().Name.Contains("CartDto"));
        }

        [TestMethod]
        public async Task ClearCart_ShouldDeleteAllItems()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId);
            var items = GetSampleEntities();
            _mockCartItemRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(items);
            _mockCartItemRepository.Setup(r => r.DeleteAsync(It.IsAny<CartItem>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.ClearCart(userId);

            // Then
            _mockCartItemRepository.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
            _mockCartItemRepository.Verify(r => r.DeleteAsync(It.IsAny<CartItem>()), Times.Exactly(items.Count));
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_OtherUser_ShouldReturnForbid()
        {
            // Given
            int userId = 1;
            int otherUserId = 2;
            int productId = 10;
            SetupHttpContext(userId);

            // When
            var result = await _controller.Delete(otherUserId, productId);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
            _mockCartItemRepository.Verify(r => r.DeleteAsync(It.IsAny<CartItem>()), Times.Never);
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldReturnForbid()
        {
            // Given
            int userId = 1;
            int otherUserId = 2;
            int productId = 10;
            SetupHttpContext(userId);

            var updateDto = new CartItemUpdateDto { Quantity = 5 };

            // When
            var result = await _controller.Put(otherUserId, productId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}