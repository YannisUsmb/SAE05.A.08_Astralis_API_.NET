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
    public class ArticlesControllerTestsMock : CrudControllerMockTests<ArticlesController, Article, ArticleListDto, ArticleDetailDto, ArticleCreateDto, ArticleUpdateDto, int>
    {
        private Mock<IArticleRepository> _mockArticleRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Rédacteur Commercial");
        }

        protected override ArticlesController CreateController(Mock<IReadableRepository<Article, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockArticleRepository = new Mock<IArticleRepository>();

            _mockCrudRepository = _mockArticleRepository.As<ICrudRepository<Article, int>>();
            _mockRepository = _mockArticleRepository.As<IReadableRepository<Article, int>>();

            return new ArticlesController(_mockArticleRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(ArticleUpdateDto dto, int id) { }

        protected override List<Article> GetSampleEntities() => new List<Article>
        {
            new Article { Id = 1, Title = "Article 1", UserId = 1 },
            new Article { Id = 2, Title = "Article 2", UserId = 1 }
        };

        protected override Article GetSampleEntity() => new Article
        {
            Id = 1,
            Title = "Article 1",
            Content = "Content 1",
            UserId = 1
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override ArticleCreateDto GetValidCreateDto() => new ArticleCreateDto
        {
            Title = "New Article",
            Content = "New Content"
        };

        protected override ArticleUpdateDto GetValidUpdateDto() => new ArticleUpdateDto
        {
            Title = "Updated Article",
            Content = "Updated Content"
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

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            int userId = 10;
            SetupHttpContext(userId);
            var createDto = GetValidCreateDto();

            _mockArticleRepository.Setup(r => r.AddAsync(It.IsAny<Article>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockArticleRepository.Verify(r => r.AddAsync(It.Is<Article>(a => a.UserId == userId)), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            int userId = 1;
            SetupHttpContext(userId);

            var updateDto = GetValidUpdateDto();
            var existingEntity = GetSampleEntity();
            existingEntity.UserId = userId;

            _mockArticleRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntity);
            _mockArticleRepository.Setup(r => r.UpdateAsync(It.IsAny<Article>(), It.IsAny<Article>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockArticleRepository.Verify(r => r.UpdateAsync(It.IsAny<Article>(), It.IsAny<Article>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
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

            _mockArticleRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockArticleRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockArticleRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
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

            _mockArticleRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntity);

            // When
            var result = await _controller.Put(id, GetValidUpdateDto());

            // Then
            _mockArticleRepository.Verify(r => r.UpdateAsync(It.IsAny<Article>(), It.IsAny<Article>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
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

            _mockArticleRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockArticleRepository.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}