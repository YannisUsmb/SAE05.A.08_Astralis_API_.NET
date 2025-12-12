using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class ArticlesControllerTests
        : CrudControllerTests<ArticlesController, Article, ArticleListDto, ArticleDetailDto, ArticleCreateDto, ArticleUpdateDto, int>
    {
        private const int TEST_EDITOR_ID = 90210;
        private const int TEST_CLIENT_ID = 90211;
        private const int TEST_ARTICLE_TYPE1_ID = 258924;
        private const int TEST_ARTICLE_TYPE2_ID = 258925;

        protected override ArticlesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var controller = new ArticlesController(new ArticleManager(context), mapper);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, TEST_EDITOR_ID.ToString()),
                new Claim(ClaimTypes.Role, "Rédacteur commercial"),
                new Claim(ClaimTypes.Name, "UserTest"),
                new Claim(ClaimTypes.Email, "email@usertest.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        protected override List<Article> GetSampleEntities()
        {
            var roleEditor = _context.UserRoles.FirstOrDefault(ur => ur.Label == "Rédacteur commercial");
            if (roleEditor == null)
            {
                roleEditor = new UserRole { Label = "Rédacteur commercial" };
                _context.UserRoles.Add(roleEditor);
                _context.SaveChanges();
            }

            var roleClient = _context.UserRoles.FirstOrDefault(ur => ur.Label == "Client");
            if (roleClient == null)
            {
                roleClient = new UserRole { Label = "Client" };
                _context.UserRoles.Add(roleClient);
                _context.SaveChanges();
            }

            var userEditor = _context.Users.FirstOrDefault(u => u.Id == TEST_EDITOR_ID);
            if (userEditor == null)
            {
                userEditor = new User
                {
                    Id = TEST_EDITOR_ID,
                    Username = "usertest",
                    UserRoleNavigation = roleEditor,
                    Email = "email@usertest.com",
                    FirstName = "User",
                    LastName = "Test",
                    Password = "Pwd",
                    IsPremium = false
                };
                if (!_context.Users.Any(u => u.Id == TEST_EDITOR_ID))
                {
                    _context.Users.Add(userEditor);
                    _context.SaveChanges();
                }
            }

            var userClient = _context.Users.FirstOrDefault(u => u.Id == TEST_CLIENT_ID);
            if (userClient == null)
            {
                userClient = new User
                {
                    Id = TEST_CLIENT_ID,
                    Username = "clienttest",
                    UserRoleNavigation = roleClient,
                    Email = "client@test.com",
                    FirstName = "Client",
                    LastName = "Test",
                    Password = "Pwd",
                    IsPremium = false
                };
                if (!_context.Users.Any(u => u.Id == TEST_CLIENT_ID))
                {
                    _context.Users.Add(userClient);
                    _context.SaveChanges();
                }
            }
            ArticleType articleType1 = new ArticleType { Id = TEST_ARTICLE_TYPE1_ID, Label = "T1" };
            ArticleType articleType2 = new ArticleType { Id = TEST_ARTICLE_TYPE2_ID, Label = "T2" };
            _context.ArticleTypes.ToList();
            _context.ArticleTypes.AddRange(articleType1, articleType2);
            _context.SaveChanges();

            var availableTypes = _context.ArticleTypes.Take(2).ToList();
            var type1 = availableTypes[0];
            var type2 = availableTypes.Count > 1 ? availableTypes[1] : availableTypes[0];

            // 6. Création des articles
            var articles = new List<Article>
            {
                new Article { Id = 902101, Title = "Article 1 title", Content = "Content Article 1", IsPremium = false, UserId = TEST_EDITOR_ID },
                new Article { Id = 902102, Title = "Article 2 title", Content = "Content Article 2", IsPremium = false, UserId = TEST_EDITOR_ID },
            };

            // Ajout Types
            articles[0].TypesOfArticle = new List<TypeOfArticle> { new TypeOfArticle { ArticleId = articles[0].Id, ArticleTypeId = articleType1.Id } };
            articles[1].TypesOfArticle = new List<TypeOfArticle> { new TypeOfArticle { ArticleId = articles[1].Id, ArticleTypeId = articleType2.Id } };

            // Ajout Intérêts
            articles[0].ArticleInterests = new List<ArticleInterest> { new ArticleInterest { ArticleId = articles[0].Id, UserId = TEST_CLIENT_ID } };
            articles[1].ArticleInterests = new List<ArticleInterest> { new ArticleInterest { ArticleId = articles[1].Id, UserId = TEST_CLIENT_ID } };

            return articles;
        }

        protected override int GetIdFromEntity(Article entity) => entity.Id;
        protected override int GetIdFromDto(ArticleDetailDto dto) => dto.Id;

        // Ajout pour ReadableControllerTests (si ta classe parente l'exige)
        // protected override int GetDtoId(ArticleListDto dto) => dto.Id;

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

        protected override void SetIdInUpdateDto(ArticleUpdateDto dto, int id)
        {
            // Vide car pas d'ID dans l'UpdateDto
        }

        [TestMethod]
        public async Task Search_ByTitle_ShouldReturnMatchingArticle()
        {
            // Given
            var filter = new ArticleFilterDto
            {
                SearchTerm = "Article 1"
            };

            // When
            var actionResult = await _controller.Search(filter);

            // Then
            Assert.IsNotNull(actionResult, "Le résultat ne devrait pas être null.");
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Devrait retourner un 200 OK.");
            var articles = okResult.Value as IEnumerable<ArticleListDto>;
            Assert.IsNotNull(articles, "La liste d'articles ne devrait pas être null.");
            Assert.AreEqual(1, articles.Count(), "Devrait trouver exactement 1 article contenant 'Article 1'.");
            Assert.AreEqual("Article 1 title", articles.First().Title);
        }

        [TestMethod]
        public async Task Search_ByTypeId_ShouldReturnSpecificArticle()
        {
            // Given
            var filter = new ArticleFilterDto
            {
                TypeIds= new List<int> { TEST_ARTICLE_TYPE1_ID }
            };

            // When
            var actionResult = await _controller.Search(filter);

            // Then
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Le résultat devrait être 200 OK");
            var articles = okResult.Value as IEnumerable<ArticleListDto>;
            Assert.IsNotNull(articles, "La liste retournée ne doit pas être null");
            Assert.AreEqual(1, articles.Count(), $"Le filtrage par le type ID {TEST_ARTICLE_TYPE1_ID} aurait dû retourner unique l'Article 1.");
            Assert.AreEqual("Article 1 title", articles.First().Title);
        }
    }
}