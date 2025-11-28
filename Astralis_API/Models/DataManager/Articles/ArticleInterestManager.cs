using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class ArticleInterestManager : CrudManager<ArticleInterest, int>, IArticleInterestRepository
    {
        public ArticleInterestManager(AstralisDbContext context) : base(context)
        {
        }
    }
}