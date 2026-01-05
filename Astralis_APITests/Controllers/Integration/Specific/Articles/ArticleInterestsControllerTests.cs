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
    public class ArticleInterestsControllerTests
        : JoinControllerTests<ArticleInterestsController, ArticleInterest, ArticleInterestDto, ArticleInterestDto, int, int>
    {
        private const int CURRENT_USER_ID = 5005;

        private const int ARTICLE_ID_1 = 100;
        private const int ARTICLE_ID_2 = 200;

        protected override ArticleInterestsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var repository = new ArticleInterestManager(context);
            var controller = new ArticleInterestsController(repository, mapper);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, CURRENT_USER_ID.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            return controller;
        }

        protected override List<ArticleInterest> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // 1. Seed User
            if (!_context.Users.Any(u => u.Id == CURRENT_USER_ID))
            {
                _context.Users.Add(new User
                {
                    Id = CURRENT_USER_ID,
                    LastName = "Fan",
                    FirstName = "Reader",
                    Email = "fan@reader.com",
                    Username = "FanReader",
                    UserRoleId = 1,
                    AvatarUrl = "url",
                    Password = "pwd"
                });
            }

            if (!_context.Articles.Any(a => a.Id == ARTICLE_ID_1))
            {
                _context.Articles.Add(new Article
                {
                    Id = ARTICLE_ID_1,
                    Title = "Article 1",
                    Content = "Contenu 1",
                    UserId = CURRENT_USER_ID
                });
            }

            if (!_context.Articles.Any(a => a.Id == ARTICLE_ID_2))
            {
                _context.Articles.Add(new Article
                {
                    Id = ARTICLE_ID_2,
                    Title = "Article 2",
                    Content = "Contenu 2",
                    UserId = CURRENT_USER_ID
                });
            }

            _context.SaveChanges();

            return new List<ArticleInterest>
            {
                new ArticleInterest
                {
                    ArticleId = ARTICLE_ID_1,
                    UserId = CURRENT_USER_ID,
                    ArticleNavigation = _context.Articles.Find(ARTICLE_ID_1)!,
                    UserNavigation = _context.Users.Find(CURRENT_USER_ID)!
                }
            };
        }


        protected override int GetKey1(ArticleInterest entity) => entity.ArticleId;
        protected override int GetKey2(ArticleInterest entity) => entity.UserId;

        protected override int GetNonExistingKey1() => 9999;
        protected override int GetNonExistingKey2() => CURRENT_USER_ID;

        protected override ArticleInterestDto GetValidCreateDto()
        {
            return new ArticleInterestDto
            {
                ArticleId = ARTICLE_ID_2,
                UserId = CURRENT_USER_ID
            };
        }

        protected override (int, int) GetKeysFromCreateDto(ArticleInterestDto dto)
        {
            return (dto.ArticleId, dto.UserId);
        }
    }
}