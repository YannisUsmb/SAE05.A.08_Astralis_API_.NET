using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class ArticleInterestInterestManager : CrudManager<ArticleInterest, int>, IArticleInterestRepository
    {
        public ArticleInterestInterestManager(AstralisDbContext context) : base(context)
        {
        }
    }
}