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

        protected override IQueryable<TypeOfArticle> WithIncludes(IQueryable<TypeOfArticle> query)
        {
            return query
                .Include(toa => toa.ArticleNavigation)
                .Include(toa => toa.ArticleTypeNavigation);
        }

        public async Task<IEnumerable<TypeOfArticle>> GetByArticleIdAsync(int userId)
        {
            return await WithIncludes(_entities)
                .Where(toa => toa.ArticleId == userId)
                .ToListAsync();
        }
        public async Task<IEnumerable<TypeOfArticle>> GetByArticleTypeIdAsync(int userId)
        {
            return await WithIncludes(_entities)
                .Where(toa => toa.ArticleTypeId == userId)
                .ToListAsync();
        }
    }
}