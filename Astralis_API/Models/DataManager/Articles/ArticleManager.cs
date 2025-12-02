using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ArticleManager : DataManager<Article, int, string>, IArticleRepository
    {
        public ArticleManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<Article?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(a => a.Id == id);
        }

        protected override IQueryable<Article> WithIncludes(IQueryable<Article> query)
        {
            return query
                .Include(a => a.TypesOfArticle)
                    .ThenInclude(toa => toa.ArticleTypeNavigation)
                .Include(a => a.ArticleInterests)
                    .ThenInclude(ai => ai.UserNavigation)
                .Include(a => a.UserNavigation)
                .Include(a => a.Comments);
        }

        public async override Task<IEnumerable<Article>> GetByKeyAsync(string title)
        {
            return await WithIncludes(_entities.Where(a => a.Title.ToLower().Contains(title.ToLower() )|| a.Content.ToLower().Contains(title.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Article>> SearchAsync(
            string? searchTerm = null,
            IEnumerable<int>? typeIds = null,
            bool? isPremium = null)
        {
            var query = _entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower();
                query = query.Where(a =>
                    a.Title.ToLower().Contains(searchTermLower) || a.Content.ToLower().Contains(searchTermLower)
                );
            }

            if (isPremium.HasValue)
                query = query.Where(a => a.IsPremium == isPremium.Value);

            if (typeIds != null && typeIds.Any())
            {
                query = query.Where(a => a.TypesOfArticle.Any(toa => typeIds.Contains(toa.ArticleTypeId)));
            }


            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}