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

        protected override IQueryable<ArticleType> WithIncludes(IQueryable<ArticleType> query)
        {
            return query
                .Include(at => at.TypesOfArticle);
        }
    }
}