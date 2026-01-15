using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class ProductsControllerTestsMock : CrudControllerMockTests<ProductsController, Product, ProductListDto, ProductDetailDto, ProductCreateDto, ProductUpdateDto, int>
    {
        private Mock<IProductRepository> _mockProductRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Rédacteur Commercial");
        }

        protected override ProductsController CreateController(Mock<IReadableRepository<Product, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockProductRepository = new Mock<IProductRepository>();

            _mockCrudRepository = _mockProductRepository.As<ICrudRepository<Product, int>>();
            _mockRepository = _mockProductRepository.As<IReadableRepository<Product, int>>();

            return new ProductsController(_mockProductRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(ProductUpdateDto dto, int id) { }

        protected override List<Product> GetSampleEntities() => new List<Product>
        {
            new Product { Id = 1, Label = "Product 1", UserId = 1 },
            new Product { Id = 2, Label = "Product 2", UserId = 1 }
        };

        protected override Product GetSampleEntity() => new Product
        {
            Id = 1,
            Label = "Product 1",
            UserId = 1,
            Price = 100
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override ProductCreateDto GetValidCreateDto() => new ProductCreateDto
        {
            Label = "New Product",
            Price = 50,
            ProductCategoryId = 1
        };

        protected override ProductUpdateDto GetValidUpdateDto() => new ProductUpdateDto
        {
            Label = "Updated Product",
            Price = 60
        };

        private void SetupHttpContext(int userId, string role = "Rédacteur Commercial")
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

        // ====================================================================
        // OVERRIDES (Adaptation logique métier : UserId issu du Token)
        // ====================================================================

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            int userId = 10;
            SetupHttpContext(userId);
            var createDto = GetValidCreateDto();

            _mockProductRepository.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockProductRepository.Verify(r => r.AddAsync(It.Is<Product>(p => p.UserId == userId)), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            int userId = 1;
            SetupHttpContext(userId); // L'utilisateur est le propriétaire

            var updateDto = GetValidUpdateDto();
            var existingEntity = GetSampleEntity();
            existingEntity.UserId = userId;

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingEntity);
            _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>(), It.IsAny<Product>())).Returns(Task.CompletedTask);

            // When
            IActionResult actionResult = await _controller.Put(id, updateDto);

            // Then
            _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>(), It.IsAny<Product>()), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            int userId = 1;
            SetupHttpContext(userId);

            var entity = GetSampleEntity();
            entity.UserId = userId;

            _mockProductRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockProductRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockProductRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_NotOwner_ShouldReturnForbid()
        {
            // Given
            int id = GetExistingId();
            int ownerId = 1;
            int attackerId = 2;

            SetupHttpContext(attackerId);

            var existingEntity = GetSampleEntity();
            existingEntity.UserId = ownerId;

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingEntity);

            // When
            IActionResult actionResult = await _controller.Put(id, GetValidUpdateDto());

            // Then
            _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>(), It.IsAny<Product>()), Times.Never);
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_NotOwner_ShouldReturnForbid()
        {
            // Given
            int id = GetExistingId();
            int ownerId = 1;
            int attackerId = 2;

            SetupHttpContext(attackerId);

            var entity = GetSampleEntity();
            entity.UserId = ownerId;

            _mockProductRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockProductRepository.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Post_InvalidUserClaims_ShouldReturnUnauthorized()
        {
            // Given
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }; // Pas de user
            var createDto = GetValidCreateDto();

            // When
            var result = await _controller.Post(createDto);

            // Then
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
            _mockProductRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }
    }
}