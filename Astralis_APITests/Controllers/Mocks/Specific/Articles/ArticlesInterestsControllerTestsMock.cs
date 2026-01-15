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
    public class ArticleInterestsControllerTestsMock : JoinControllerMockTests<ArticleInterestsController, ArticleInterest, ArticleInterestDto, ArticleInterestDto, int, int>
    {
        private Mock<IArticleInterestRepository> _mockArticleInterestRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override ArticleInterestsController CreateController(Mock<IJoinRepository<ArticleInterest, int, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockArticleInterestRepository = new Mock<IArticleInterestRepository>();
            _mockRepository = _mockArticleInterestRepository.As<IJoinRepository<ArticleInterest, int, int>>();

            return new ArticleInterestsController(_mockArticleInterestRepository.Object, mapper);
        }

        protected override List<ArticleInterest> GetSampleEntities() => new List<ArticleInterest>
        {
            new ArticleInterest { ArticleId = 1, UserId = 1 },
            new ArticleInterest { ArticleId = 2, UserId = 1 }
        };

        protected override ArticleInterest GetSampleEntity() => new ArticleInterest
        {
            ArticleId = 1,
            UserId = 1
        };

        protected override int GetExistingKey1() => 1;
        protected override int GetExistingKey2() => 1;

        protected override int GetNonExistingKey1() => 999;
        protected override int GetNonExistingKey2() => 999;

        protected override ArticleInterestDto GetValidCreateDto() => new ArticleInterestDto
        {
            ArticleId = 3,
            UserId = 1
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
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId);
            var createDto = GetValidCreateDto();

            _mockArticleInterestRepository.Setup(r => r.AddAsync(It.IsAny<ArticleInterest>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockArticleInterestRepository.Verify(r => r.AddAsync(It.Is<ArticleInterest>(x => x.UserId == userId)), Times.Once);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int articleId = 1;
            int userId = 1;
            SetupHttpContext(userId);
            var entity = GetSampleEntity();

            _mockArticleInterestRepository.Setup(r => r.GetByIdAsync(articleId, userId)).ReturnsAsync(entity);
            _mockArticleInterestRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(articleId, userId);

            // Then
            _mockArticleInterestRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_NonExistingIds_ShouldReturnNotFound()
        {
            // Given
            int id1 = GetNonExistingKey1();
            int id2 = GetNonExistingKey2();

            SetupHttpContext(id2);

            _mockArticleInterestRepository.Setup(r => r.GetByIdAsync(id1, id2)).ReturnsAsync((ArticleInterest)null);

            // When
            var result = await _controller.Delete(id1, id2);

            // Then
            _mockArticleInterestRepository.Verify(r => r.GetByIdAsync(id1, id2), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Post_UserIdMismatch_ShouldReturnBadRequest()
        {
            // Given
            int authUserId = 1;
            int otherUserId = 2;
            SetupHttpContext(authUserId);
            var dto = new ArticleInterestDto { ArticleId = 5, UserId = otherUserId };

            // When
            var result = await _controller.Post(dto);

            // Then
            _mockArticleInterestRepository.Verify(r => r.AddAsync(It.IsAny<ArticleInterest>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Delete_UserIdMismatch_ShouldReturnBadRequest()
        {
            // Given
            int authUserId = 1;
            int otherUserId = 2;
            int articleId = 5;
            SetupHttpContext(authUserId);

            // When
            var result = await _controller.Delete(articleId, otherUserId);

            // Then
            _mockArticleInterestRepository.Verify(r => r.DeleteAsync(It.IsAny<ArticleInterest>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}