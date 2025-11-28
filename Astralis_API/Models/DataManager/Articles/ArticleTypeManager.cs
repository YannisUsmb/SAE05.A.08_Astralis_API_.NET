using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class ArticleTypeManager : CrudManager<ArticleType, int>, IArticleTypeRepository
    {
        public ArticleTypeManager(AstralisDbContext context) : base(context)
        {
        }
    }
}       