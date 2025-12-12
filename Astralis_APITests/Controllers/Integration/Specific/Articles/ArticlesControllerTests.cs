using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class ArticlesControllerTests
        : CrudControllerTests<ArticlesController, Article, ArticleListDto, ArticleDetailDto, ArticleCreateDto, ArticleUpdateDto, int>
    {
        private const int TEST_EDITOR_ID = 90210;
        private const int TEST_CLIENT_ID = 90211;

        protected override ArticlesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var controller = new ArticlesController(new ArticleManager(context), mapper);

            // --- SIMULATION DU JWT DÉCRYPTÉ ---
            var claims = new List<Claim>
            {
                // L'ID doit matcher celui de l'auteur de l'article en BDD
                new Claim(ClaimTypes.NameIdentifier, TEST_EDITOR_ID.ToString()),
                new Claim(ClaimTypes.Role, "Rédacteur commercial"),
                new Claim(ClaimTypes.Name, "UserTest"),
                new Claim(ClaimTypes.Email, "email@usertest.com")
            };

            // "TestAuth" permet de mettre IsAuthenticated = true
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
            ArticleType articleType1 = new ArticleType { Id = 258924, Label = "T1" };
            ArticleType articleType2 = new ArticleType { Id = 258925, Label = "T2" };
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
            // Arrange
            // Dans GetSampleEntities, nous avons créé "Article 1 title" et "Article 2 title"
            var filter = new ArticleFilterDto
            {
                SearchTerm = "Article 1"
            };

            // Act
            // Note: Search retourne ActionResult<IEnumerable<ArticleListDto>>
            var actionResult = await _controller.Search(filter);

            // Assert
            Assert.IsNotNull(actionResult, "Le résultat ne devrait pas être null.");

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Devrait retourner un 200 OK.");

            var articles = okResult.Value as IEnumerable<ArticleListDto>;
            Assert.IsNotNull(articles, "La liste d'articles ne devrait pas être null.");

            // On s'attend à trouver exactement 1 article correspondant
            Assert.AreEqual(1, articles.Count(), "Devrait trouver exactement 1 article contenant 'Article 1'.");
            Assert.AreEqual("Article 1 title", articles.First().Title);
        }

        [TestMethod]
        public async Task Search_ByTypeId_ShouldReturnSpecificArticle()
        {
            var article1 = _context.Articles
                .Include(a => a.TypesOfArticle)
                .FirstOrDefault(a => a.Id == 902101);

            var article2 = _context.Articles
                .Include(a => a.TypesOfArticle)
                .FirstOrDefault(a => a.Id == 902102);

            Assert.IsNotNull(article1, "L'article 1 devrait exister via le Seed.");
            Assert.IsNotNull(article2, "L'article 2 devrait exister via le Seed.");
            var distinctTypeId = article1.TypesOfArticle
                .Select(toa => toa.ArticleTypeId)
                .Except(article2.TypesOfArticle.Select(toa => toa.ArticleTypeId))
                .FirstOrDefault();
            if (distinctTypeId == 0)
            {
                Assert.Inconclusive("Test ignoré : Pas assez de types différents en BDD pour distinguer l'article 1 de l'article 2.");
                return;
            }

            var filter = new ArticleFilterDto
            {
                TypeIds = new List<int> { distinctTypeId }
            };

            // Act
            var actionResult = await _controller.Search(filter);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Le résultat devrait être 200 OK");

            var articles = okResult.Value as IEnumerable<ArticleListDto>;
            Assert.IsNotNull(articles, "La liste retournée ne doit pas être null");

            Assert.AreEqual(1, articles.Count(), $"Le filtrage par le type ID {distinctTypeId} aurait dû retourner unique l'Article 1.");
            Assert.AreEqual("Article 1 title", articles.First().Title);
        }
    }
}