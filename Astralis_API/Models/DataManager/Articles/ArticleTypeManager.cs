using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class ArticleTypeTypeManager : CrudManager<ArticleType, int>, IArticleTypeRepository
    {
        public ArticleTypeTypeManager(AstralisDbContext context) : base(context)
        {
        }
    }
}       