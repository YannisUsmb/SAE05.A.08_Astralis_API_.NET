using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class ArticlesControllerTests
        : CrudControllerTests<ArticlesController, Article, ArticleListDto, ArticleDetailDto, ArticleCreateDto, ArticleUpdateDto, int>
    {
        protected override ArticlesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new ArticlesController(new ArticleManager(context), mapper);
        }

        protected override List<Article> GetSampleEntities()
        {
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.Label == "Rédacteur Commercial");
            var user = _context.Users.FirstOrDefault(c => c.UserRoleNavigation.Label == "Rédacteur Commercial");
            if (user == null)
            {
                user = new User { Id = 90210, Username = "usertest", UserRoleNavigation = userRole, Email = "email@usertest.com", FirstName = "UserTest firstname", LastName = "UserTest lastname", Password = "UserTestPassword12.", AvatarUrl = "https://test.com/useravatar.png" };
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            /*ArticleType articletype1 = _context.ArticleTypes.FirstOrDefault(at => at.Label == "Événement rare");
            ArticleType articletype2 = _context.ArticleTypes.FirstOrDefault(at => at.Label == "Événement rare");
            List<ArticleType> articletypes = new List<ArticleType> { articletype1, articletype2 };*/
            var articles = new List<Article>
            {
                new Article { Id = 902101, Title = "Article 1 title", Content = "Content Article 1", IsPremium = false, UserNavigation = user,  },
                new Article { Id = 902102, Title = "Article 2 title", Content = "Content Article 2", IsPremium = false, UserNavigation = user},
            };

            /*articles[1].TypesOfArticle = new List<TypeOfArticle>
            {
                new TypeOfArticle{ArticleId=articles[1].Id, ArticleTypeId =articletype1.Id },
                new TypeOfArticle{ArticleId=articles[1].Id, ArticleTypeId =articletype2.Id }

            };
            articles[2].TypesOfArticle = new List<TypeOfArticle>
            {
                new TypeOfArticle{ArticleId=articles[1].Id, ArticleTypeId =articletype1.Id },
                new TypeOfArticle{ArticleId=articles[1].Id, ArticleTypeId =articletype2.Id }

            };*/

            return articles;
        }

        protected override int GetIdFromEntity(Article entity)
        {
            return entity.Id;
        }

        protected override int GetIdFromDto(ArticleDetailDto dto)
        {
            return dto.Id;
        }

        protected override int GetNonExistingId()
        {
            return 9999999;
        }

        protected override ArticleCreateDto GetValidCreateDto()
        {
            int cityId = _context.Cities.FirstOrDefault(c => c.Name == "ANNECY").Id;
            return new ArticleCreateDto
            {
                Title = "Article 1 title",
                Content = "Content Article 1",
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
        }
    }
}