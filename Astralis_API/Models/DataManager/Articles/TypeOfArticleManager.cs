using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class TypeOfArticleManager : JoinManager<TypeOfArticle, int, int>, ITypeOfArticleRepository
    {
        public TypeOfArticleManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<TypeOfArticle?> GetByIdAsync(int articleTypeId, int articleId)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(toa => toa.ArticleTypeId == articleTypeId && toa.ArticleId == articleId);
        }

        protected override IQueryable<TypeOfArticle> WithIncludes(IQueryable<TypeOfArticle> query)
        {
            return query
                .Include(toa => toa.ArticleNavigation)
                .Include(toa => toa.ArticleTypeNavigation);
        }
    }
}