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
    public class CommentsControllerTestsMock : CrudControllerMockTests<CommentsController, Comment, CommentDto, CommentDto, CommentCreateDto, CommentUpdateDto, int>
    {
        private Mock<ICommentRepository> _mockCommentRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override CommentsController CreateController(Mock<IReadableRepository<Comment, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockCommentRepository = new Mock<ICommentRepository>();

            _mockCrudRepository = _mockCommentRepository.As<ICrudRepository<Comment, int>>();
            _mockRepository = _mockCommentRepository.As<IReadableRepository<Comment, int>>();

            return new CommentsController(_mockCommentRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(CommentUpdateDto dto, int id)
        {
        }


        protected override List<Comment> GetSampleEntities() => new List<Comment>
        {
            new Comment
            {
                Id = 1,
                Text = "Super article !",
                UserId = 1,
                ArticleId = 10,
                Date = DateTime.UtcNow,
                UserNavigation = new User { Username = "User1" }
            },
            new Comment
            {
                Id = 2,
                Text = "Je ne suis pas d'accord.",
                UserId = 2,
                ArticleId = 10,
                Date = DateTime.UtcNow.AddMinutes(5),
                UserNavigation = new User { Username = "User2" }
            }
        };

        protected override Comment GetSampleEntity() => new Comment
        {
            Id = 1,
            Text = "Super article !",
            UserId = 1,
            ArticleId = 10,
            Date = DateTime.UtcNow,
            IsVisible = true,
            UserNavigation = new User { Username = "User1" }
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override CommentCreateDto GetValidCreateDto() => new CommentCreateDto
        {
            ArticleId = 10,
            Text = "Nouveau commentaire",
            RepliesToId = null
        };

        protected override CommentUpdateDto GetValidUpdateDto() => new CommentUpdateDto
        {
            Text = "Commentaire édité"
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
        public new async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            int userId = 1;
            SetupHttpContext(userId);
            var createDto = GetValidCreateDto();
            _mockCommentRepository.Setup(r => r.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
            _mockCommentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetSampleEntity());

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockCommentRepository.Verify(r => r.AddAsync(It.Is<Comment>(c => c.UserId == userId && c.ArticleId == createDto.ArticleId)), Times.Once);
            _mockCommentRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public new async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            int userId = 1;
            SetupHttpContext(userId);
            var updateDto = GetValidUpdateDto();
            var entity = GetSampleEntity();
            entity.UserId = userId;

            _mockCommentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockCommentRepository.Setup(r => r.UpdateAsync(entity, entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockCommentRepository.Verify(r => r.UpdateAsync(entity, entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            var dto = okResult.Value as CommentDto;
            Assert.AreEqual(updateDto.Text, entity.Text);
        }

        [TestMethod]
        public new async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {

            // Given
            int id = GetExistingId();
            int userId = 1;
            SetupHttpContext(userId);
            var entity = GetSampleEntity();
            entity.UserId = userId;

            _mockCommentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockCommentRepository.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockCommentRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }


        [TestMethod]
        public async Task GetByArticleId_ShouldReturnTreeStructure()
        {
            // Given
            int articleId = 10;
            var parent = new Comment { Id = 1, ArticleId = articleId, Text = "Parent", Date = DateTime.UtcNow, UserNavigation = new User() };
            var child = new Comment { Id = 2, ArticleId = articleId, Text = "Enfant", RepliesToId = 1, Date = DateTime.UtcNow.AddMinutes(1), UserNavigation = new User() };

            var flatList = new List<Comment> { parent, child };

            _mockCommentRepository.Setup(r => r.GetByArticleIdAsync(articleId)).ReturnsAsync(flatList);

            // When
            var result = await _controller.GetByArticleId(articleId);

            // Then
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var tree = okResult.Value as List<CommentDto>;

            Assert.IsNotNull(tree);
            Assert.AreEqual(1, tree.Count, "Il ne devrait y avoir qu'une seule racine (le parent).");
            Assert.AreEqual(1, tree[0].Replies.Count, "Le parent devrait avoir une réponse.");
            Assert.AreEqual("Enfant", tree[0].Replies[0].Text);
        }

        [TestMethod]
        public async Task Put_NotOwner_ShouldReturnForbid()
        {
            // Given
            int id = GetExistingId();
            int ownerId = 1;
            int attackerId = 2;
            SetupHttpContext(attackerId);

            var entity = GetSampleEntity();
            entity.UserId = ownerId;

            _mockCommentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            // When
            var result = await _controller.Put(id, GetValidUpdateDto());

            // Then
            _mockCommentRepository.Verify(r => r.UpdateAsync(It.IsAny<Comment>(), It.IsAny<Comment>()), Times.Never);
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

            _mockCommentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockCommentRepository.Verify(r => r.DeleteAsync(It.IsAny<Comment>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_AsAdmin_ShouldBeAllowed()
        {
            // Given
            int id = GetExistingId();
            int ownerId = 1;
            SetupHttpContext(99, "Admin");

            var entity = GetSampleEntity();
            entity.UserId = ownerId;

            _mockCommentRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockCommentRepository.Setup(r => r.UpdateAsync(entity, entity)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, GetValidUpdateDto());

            // Then
            _mockCommentRepository.Verify(r => r.UpdateAsync(entity, entity), Times.Once);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}