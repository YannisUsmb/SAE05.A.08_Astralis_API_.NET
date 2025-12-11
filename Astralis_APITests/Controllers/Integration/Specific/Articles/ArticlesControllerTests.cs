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
            // 1. Gestion du Rôle Rédacteur
            var roleEditor = _context.UserRoles.FirstOrDefault(ur => ur.Label == "Rédacteur commercial");
            if (roleEditor == null)
            {
                roleEditor = new UserRole { Label = "Rédacteur commercial" };
                _context.UserRoles.Add(roleEditor);
                _context.SaveChanges();
            }

            // 2. Gestion du Rôle Client
            var roleClient = _context.UserRoles.FirstOrDefault(ur => ur.Label == "Client");
            if (roleClient == null)
            {
                roleClient = new UserRole { Label = "Client" };
                _context.UserRoles.Add(roleClient);
                _context.SaveChanges();
            }

            // 3. Création du User Rédacteur (ID 90210)
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

            // 4. Création du User Client (ID 90211) pour les intérêts
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

            // 5. Gestion des Types d'articles
            if (!_context.ArticleTypes.Any())
            {
                _context.ArticleTypes.AddRange(new ArticleType { Label = "T1" }, new ArticleType { Label = "T2" });
                _context.SaveChanges();
            }
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
            articles[0].TypesOfArticle = new List<TypeOfArticle> { new TypeOfArticle { ArticleId = articles[0].Id, ArticleTypeId = type1.Id } };
            if (type1.Id != type2.Id) articles[0].TypesOfArticle.Add(new TypeOfArticle { ArticleId = articles[0].Id, ArticleTypeId = type2.Id });

            articles[1].TypesOfArticle = new List<TypeOfArticle> { new TypeOfArticle { ArticleId = articles[1].Id, ArticleTypeId = type1.Id } };

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
    }
}