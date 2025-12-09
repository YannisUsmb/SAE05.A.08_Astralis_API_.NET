using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class ArticleTypesControllerTests : ReadableControllerTests<ArticleTypesController, ArticleType, ArticleTypeDto, ArticleTypeDto, int>
    {
        protected override ArticleTypesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new ArticleTypesController(new ArticleTypeManager(context), mapper);
        }

        protected override List<ArticleType> GetSampleEntities()
        {
            return new List<ArticleType>
            {
                new ArticleType {Id=902101, Label = "Article type 1" },
                new ArticleType {Id=902102, Label = "Article type 2" }
            };
        }

        protected override int GetIdFromEntity(ArticleType entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}