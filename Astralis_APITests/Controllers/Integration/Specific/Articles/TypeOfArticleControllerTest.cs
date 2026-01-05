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
    public class TypeOfArticlesControllerTests
        : JoinControllerTests<TypesOfArticleController, TypeOfArticle, TypeOfArticleDto, TypeOfArticleDto, int, int>
    {
        private const int USER_ID = 5005;
        private const int TYPE_ID_A = 10;
        private const int TYPE_ID_B = 20;
        private const int ARTICLE_ID_1 = 100;
        private const int ARTICLE_ID_2 = 200;

        protected override TypesOfArticleController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var repository = new TypeOfArticleManager(context);
            var controller = new TypesOfArticleController(repository, mapper);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, USER_ID.ToString()),
                new Claim(ClaimTypes.Role, "Rédacteur commercial")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            return controller;
        }

        protected override List<TypeOfArticle> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            if (!_context.Users.Any(u => u.Id == USER_ID))
                _context.Users.Add(new User { Id = USER_ID, LastName = "Ed", FirstName = "Itor", Email = "e@d.com", Username = "Editor", UserRoleId = 2, AvatarUrl = "url", Password = "pwd" });

            if (!_context.ArticleTypes.Any(t => t.Id == TYPE_ID_A))
                _context.ArticleTypes.Add(new ArticleType { Id = TYPE_ID_A, Label = "News" });

            if (!_context.ArticleTypes.Any(t => t.Id == TYPE_ID_B))
                _context.ArticleTypes.Add(new ArticleType { Id = TYPE_ID_B, Label = "Tech" });

            if (!_context.Articles.Any(a => a.Id == ARTICLE_ID_1))
                _context.Articles.Add(new Article
                {
                    Id = ARTICLE_ID_1,
                    Title = "A1",
                    Content = "Contenu de l'article 1", // <--- Obligatoire
                    UserId = USER_ID
                });

            if (!_context.Articles.Any(a => a.Id == ARTICLE_ID_2))
                _context.Articles.Add(new Article
                {
                    Id = ARTICLE_ID_2,
                    Title = "A2",
                    Content = "Contenu de l'article 2", // <--- Obligatoire
                    UserId = USER_ID
                });

            _context.SaveChanges();

            return new List<TypeOfArticle>
            {
                new TypeOfArticle
                {
                    ArticleTypeId = TYPE_ID_A,
                    ArticleId = ARTICLE_ID_1,
                    ArticleTypeNavigation = _context.ArticleTypes.Find(TYPE_ID_A)!,
                    ArticleNavigation = _context.Articles.Find(ARTICLE_ID_1)!
                }
            };
        }

        protected override int GetKey1(TypeOfArticle entity) => entity.ArticleTypeId;
        protected override int GetKey2(TypeOfArticle entity) => entity.ArticleId;
        protected override int GetNonExistingKey1() => 999;
        protected override int GetNonExistingKey2() => 999;

        protected override TypeOfArticleDto GetValidCreateDto()
        {
            return new TypeOfArticleDto
            {
                ArticleTypeId = TYPE_ID_B,
                ArticleId = ARTICLE_ID_2
            };
        }

        protected override (int, int) GetKeysFromCreateDto(TypeOfArticleDto dto)
        {
            return (dto.ArticleTypeId, dto.ArticleId);
        }
    }
}