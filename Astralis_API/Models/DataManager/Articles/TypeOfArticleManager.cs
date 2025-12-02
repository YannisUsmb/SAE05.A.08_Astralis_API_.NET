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
    }
}