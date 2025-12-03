using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ArticleInterestManager : JoinManager<ArticleInterest, int, int>, IArticleInterestRepository
    {
        public ArticleInterestManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<ArticleInterest?> GetByIdAsync(int articleId, int userId)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(ai => ai.ArticleId == articleId && ai.UserId == userId);
        }

        protected override IQueryable<ArticleInterest> WithIncludes(IQueryable<ArticleInterest> query)
        {
            return query
                .Include(ai => ai.ArticleNavigation)
                .Include(ai => ai.UserNavigation);
        }

        public async Task<IEnumerable<ArticleInterest>> GetByUserIdAsync(int userId)
        {
            return await WithIncludes(_entities)
                .Where(ai => ai.UserId == userId)
                .ToListAsync();
        }
    }
}