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
    public class ArticlesControllerTests
        : CrudControllerTests<ArticlesController, Article, ArticleListDto, ArticleDetailDto, ArticleCreateDto, ArticleUpdateDto, int>
    {
        private const int TEST_EDITOR_ID = 90210;
        private const int TEST_OTHER_EDITOR_ID = 90299;
        private const int TEST_CLIENT_ID = 90211;
        private const int TEST_ARTICLE_TYPE1_ID = 258924;
        private const int TEST_ARTICLE_TYPE2_ID = 258925;

        private int _article1Id;
        private int _article2Id;

        protected override ArticlesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var controller = new ArticlesController(new ArticleManager(context), mapper);
            SetupUserContext(controller, TEST_EDITOR_ID, "Rédacteur commercial");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, "TestUser_" + userId),
                new Claim(ClaimTypes.Email, $"test{userId}@test.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<Article> GetSampleEntities()
        {
            var roleEditor = GetOrCreateRole("Rédacteur commercial");
            var roleClient = GetOrCreateRole("Client");

            CreateUserIfNotExist(TEST_EDITOR_ID, "EditorUser", roleEditor.Id);
            CreateUserIfNotExist(TEST_OTHER_EDITOR_ID, "OtherEditor", roleEditor.Id);
            CreateUserIfNotExist(TEST_CLIENT_ID, "ClientUser", roleClient.Id);

            var t1 = GetOrCreateType(TEST_ARTICLE_TYPE1_ID, "T1");
            var t2 = GetOrCreateType(TEST_ARTICLE_TYPE2_ID, "T2");

            _context.SaveChanges();

            var articles = new List<Article>();

            var a1 = new Article
            {
                Id = 902101,
                Title = "Article 1 title",
                Content = "Content Article 1",
                IsPremium = false,
                UserId = TEST_EDITOR_ID
            };
            a1.TypesOfArticle = new List<TypeOfArticle> { new TypeOfArticle { ArticleTypeId = t1.Id } };
            a1.ArticleInterests = new List<ArticleInterest> { new ArticleInterest { UserId = TEST_CLIENT_ID } };

            if (!_context.Articles.Any(a => a.Id == a1.Id)) articles.Add(a1);
            _article1Id = a1.Id;

            var a2 = new Article
            {
                Id = 902102,
                Title = "Article 2 title",
                Content = "Content Article 2",
                IsPremium = false,
                UserId = TEST_EDITOR_ID
            };
            a2.TypesOfArticle = new List<TypeOfArticle> { new TypeOfArticle { ArticleTypeId = t2.Id } };
            a2.ArticleInterests = new List<ArticleInterest> { new ArticleInterest { UserId = TEST_CLIENT_ID } };

            if (!_context.Articles.Any(a => a.Id == a2.Id)) articles.Add(a2);
            _article2Id = a2.Id;

            return articles;
        }

        private UserRole GetOrCreateRole(string label)
        {
            var r = _context.UserRoles.FirstOrDefault(x => x.Label == label);
            if (r == null) { r = new UserRole { Label = label }; _context.UserRoles.Add(r); _context.SaveChanges(); }
            return r;
        }

        private void CreateUserIfNotExist(int id, string username, int roleId)
        {
            if (!_context.Users.Any(u => u.Id == id))
            {
                var role = _context.UserRoles.Find(roleId);
                _context.Users.Add(new User
                {
                    Id = id,
                    Username = username,
                    UserRoleNavigation = role!,
                    Email = $"{username}@test.com",
                    FirstName = username,
                    LastName = "Test",
                    Password = "Pwd",
                    IsPremium = false
                });
                _context.SaveChanges();
            }
        }

        private ArticleType GetOrCreateType(int id, string label)
        {
            var t = _context.ArticleTypes.FirstOrDefault(x => x.Id == id);
            if (t == null) { t = new ArticleType { Id = id, Label = label }; _context.ArticleTypes.Add(t); _context.SaveChanges(); }
            return t;
        }

        protected override int GetIdFromEntity(Article entity) => entity.Id;
        protected override int GetIdFromDto(ArticleDetailDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override ArticleCreateDto GetValidCreateDto()
        {
            return new ArticleCreateDto
            {
                Title = "New Article Title",
                Content = "Content Article New",
                IsPremium = false
            };
        }

        protected override ArticleUpdateDto GetValidUpdateDto(Article entityToUpdate)
        {
            return new ArticleUpdateDto
            {
                Title = entityToUpdate.Title + " Updated",
                Content = entityToUpdate.Content,
                IsPremium = entityToUpdate.IsPremium
            };
        }

        protected override void SetIdInUpdateDto(ArticleUpdateDto dto, int id) { }

        [TestMethod]
        public async Task Search_ByTitle_ShouldReturnMatchingArticle()
        {
            // Given
            var filter = new ArticleFilterDto { SearchTerm = "Article 1" };

            // When
            var actionResult = await _controller.Search(filter);

            // Then
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var resultValue = okResult.Value;
            var itemsProperty = resultValue.GetType().GetProperty("Items");
            Assert.IsNotNull(itemsProperty, "La propriété 'Items' est introuvable dans le résultat de Search.");

            var articles = itemsProperty.GetValue(resultValue) as IEnumerable<ArticleListDto>;
            Assert.IsNotNull(articles);
            Assert.AreEqual(1, articles.Count());
            Assert.AreEqual("Article 1 title", articles.First().Title);
        }

        [TestMethod]
        public async Task Search_ByTypeId_ShouldReturnSpecificArticle()
        {
            // Given
            var filter = new ArticleFilterDto { TypeIds = new List<int> { TEST_ARTICLE_TYPE1_ID } };

            // When
            var actionResult = await _controller.Search(filter);

            // Then
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultValue = okResult.Value;
            var itemsProperty = resultValue.GetType().GetProperty("Items");
            var articles = itemsProperty.GetValue(resultValue) as IEnumerable<ArticleListDto>;

            Assert.IsNotNull(articles);
            Assert.AreEqual(1, articles.Count());
            Assert.AreEqual("Article 1 title", articles.First().Title);
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            // Given
            var createDto = GetValidCreateDto();

            // When
            var actionResult = await _controller.Post(createDto);

            // Then
            Assert.IsInstanceOfType(actionResult.Result, typeof(CreatedAtActionResult));

            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var resultDto = createdResult.Value as ArticleDetailDto;
            Assert.IsNotNull(resultDto);

            _context.ChangeTracker.Clear();
            var createdEntity = await _context.Articles.FirstOrDefaultAsync(a => a.Title == createDto.Title);
            Assert.IsNotNull(createdEntity);
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn201()
        {
            // Given
            var createDto = GetValidCreateDto();

            // When
            var actionResult = await _controller.Post(createDto);

            // Then
            Assert.IsInstanceOfType(actionResult.Result, typeof(CreatedAtActionResult));

            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var resultDto = createdResult.Value as ArticleDetailDto;
            Assert.IsNotNull(resultDto);

            _context.ChangeTracker.Clear();
            var createdEntity = await _context.Articles.FirstOrDefaultAsync(a => a.Title == createDto.Title);
            Assert.IsNotNull(createdEntity);
        }

        [TestMethod]
        public async Task Put_UpdateContent_AsOwner_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_EDITOR_ID, "Rédacteur commercial");
            var updateDto = new ArticleUpdateDto
            {
                Title = "Updated Title By Owner",
                Content = "Updated Content",
                IsPremium = true
            };

            // When
            var actionResult = await _controller.Put(_article1Id, updateDto);

            // Then
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updatedEntity = await _context.Articles.FindAsync(_article1Id);
            Assert.AreEqual("Updated Title By Owner", updatedEntity!.Title);
            Assert.IsTrue(updatedEntity.IsPremium);
        }

        [TestMethod]
        public async Task Put_AsOtherEditor_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_OTHER_EDITOR_ID, "Rédacteur commercial");
            var updateDto = new ArticleUpdateDto
            {
                Title = "Hacked Title",
                Content = "Hacked Content",
                IsPremium = false
            };

            // When
            var actionResult = await _controller.Put(_article1Id, updateDto);

            // Then
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));

            _context.ChangeTracker.Clear();
            var entity = await _context.Articles.FindAsync(_article1Id);
            Assert.AreNotEqual("Hacked Title", entity!.Title, "Le titre ne doit pas avoir changé.");
        }

        [TestMethod]
        public async Task Delete_AsOwner_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_EDITOR_ID, "Rédacteur commercial");

            // When
            var actionResult = await _controller.Delete(_article1Id);

            // Then
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var deletedEntity = await _context.Articles.FindAsync(_article1Id);
            Assert.IsNull(deletedEntity, "L'article devrait avoir été supprimé.");
        }

        [TestMethod]
        public async Task Delete_AsOtherEditor_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_OTHER_EDITOR_ID, "Rédacteur commercial");

            // When
            var actionResult = await _controller.Delete(_article2Id);

            // Then
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));

            _context.ChangeTracker.Clear();
            var entity = await _context.Articles.FindAsync(_article2Id);
            Assert.IsNotNull(entity, "L'article ne devrait PAS avoir été supprimé.");
        }
    }
}