using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class CommentsControllerTests
        : CrudControllerTests<CommentsController, Comment, CommentDto, CommentDto, CommentCreateDto, CommentUpdateDto, int>
    {
        private const int USER_OWNER_ID = 5001;
        private const int USER_ADMIN_ID = 5002;
        private const int USER_STRANGER_ID = 5003;

        private const int ARTICLE_ID = 88001;
        private const int COMMENT_ID_1 = 77001;
        private const int COMMENT_ID_2 = 77002;

        private int _commentOwnerId;
        private int _commentStrangerId;

        protected override CommentsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var commentManager = new CommentManager(context);

            var controller = new CommentsController(
                commentManager,
                mapper
            );

            SetupUserContext(controller, USER_OWNER_ID, "User");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, $"User_{userId}")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<Comment> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();


            CreateUserIfNotExist(USER_OWNER_ID, 1);
            CreateUserIfNotExist(USER_ADMIN_ID, 2);
            CreateUserIfNotExist(USER_STRANGER_ID, 1);

            if (!_context.Articles.AsNoTracking().Any(a => a.Id == ARTICLE_ID))
            {
                _context.Articles.Add(new Article
                {
                    Id = ARTICLE_ID,
                    Title = "Test Article",
                    Content = "Content",
                    UserId = USER_ADMIN_ID,
                });

            }

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            var c1 = CreateCommentInMemory(
                COMMENT_ID_1,
                "This is a comment by Owner",
                USER_OWNER_ID,
                ARTICLE_ID
            );

            var c2 = CreateCommentInMemory(
                COMMENT_ID_2,
                "This is a comment by Stranger",
                USER_STRANGER_ID,
                ARTICLE_ID
            );

            _commentOwnerId = COMMENT_ID_1;
            _commentStrangerId = COMMENT_ID_2;

            return new List<Comment> { c1, c2 };
        }

        private Comment CreateCommentInMemory(int id, string text, int userId, int articleId)
        {
            return new Comment
            {
                Id = id,
                UserId = userId,
                ArticleId = articleId,
                Text = text,
                Date = DateTime.UtcNow,
                IsVisible = true,
                RepliesToId = null
            };
        }

        private void CreateUserIfNotExist(int id, int roleId)
        {
            if (!_context.Users.AsNoTracking().Any(u => u.Id == id))
            {
                _context.Users.Add(new User
                {
                    Id = id,
                    UserRoleId = roleId,
                    Username = $"User{id}",
                    Email = $"user{id}@test.com",
                    FirstName = "Test",
                    LastName = "User",
                    Password = "Pwd"
                });
            }
        }

        protected override int GetIdFromEntity(Comment entity) => entity.Id;
        protected override int GetIdFromDto(CommentDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override CommentCreateDto GetValidCreateDto()
        {
            return new CommentCreateDto
            {
                ArticleId = ARTICLE_ID,
                Text = "New Created Comment",
                RepliesToId = null
            };
        }

        protected override CommentUpdateDto GetValidUpdateDto(Comment entityToUpdate)
        {
            return new CommentUpdateDto
            {
                Text = "Updated Text Content"
            };
        }

        protected override void SetIdInUpdateDto(CommentUpdateDto dto, int id) { }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "User");

            await base.Post_ValidObject_ShouldCreateAndReturn200();
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            await Post_ValidObject_ShouldCreateAndReturn200();
        }


        [TestMethod]
        public async Task Delete_Owner_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "User");

            var result = await _controller.Delete(_commentOwnerId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var deleted = await _context.Comments.FindAsync(_commentOwnerId);
            Assert.IsNull(deleted, "Le commentaire devrait être supprimé de la base.");
        }

        [TestMethod]
        public async Task Delete_Stranger_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "User");

            var result = await _controller.Delete(_commentStrangerId);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_Admin_ShouldSuccess_AnyComment()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");

            var result = await _controller.Delete(_commentOwnerId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_Owner_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_OWNER_ID, "User");
            var updateDto = GetValidUpdateDto(new Comment());

            var result = await _controller.Put(_commentOwnerId, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updated = await _context.Comments.FindAsync(_commentOwnerId);
            Assert.AreEqual("Updated Text Content", updated.Text);
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var result = await _controller.GetById(_commentOwnerId);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as CommentDto;
            Assert.AreEqual(_commentOwnerId, dto.Id);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<CommentDto>;
            Assert.IsTrue(list.Any(c => c.Id == _commentOwnerId));
        }
    }
}