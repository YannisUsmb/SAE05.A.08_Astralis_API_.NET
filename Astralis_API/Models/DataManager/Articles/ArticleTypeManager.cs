using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ArticleTypeManager : ReadableManager<ArticleType, int>, IArticleTypeRepository
    {
        public ArticleTypeManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<ArticleType?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(at => at.Id == id);
        }

        protected override IQueryable<ArticleType> WithIncludes(IQueryable<ArticleType> query)
        {
            return query
                .Include(at => at.TypesOfArticle)
                    .ThenInclude(toa =>toa.ArticleNavigation);
        }
    }
}