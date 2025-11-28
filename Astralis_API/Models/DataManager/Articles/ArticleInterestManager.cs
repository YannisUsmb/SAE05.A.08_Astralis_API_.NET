using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ArticleInterestManager : CrudManager<ArticleInterest, int>, IArticleInterestRepository
    {
        public ArticleInterestManager(AstralisDbContext context) : base(context)
        {
        }
        protected override IQueryable<ArticleInterest> WithIncludes(IQueryable<ArticleInterest> query)
        {
            return query
                .Include(ai => ai.ArticleNavigation)
                .Include(ai => ai.UserNavigation);
        }
    }
}