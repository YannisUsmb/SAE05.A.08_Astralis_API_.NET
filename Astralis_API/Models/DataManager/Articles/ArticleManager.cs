using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ArticleManager : DataManager<Article, int, string>, IArticleRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Article> _articles;

        public ArticleManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _articles = _context.Set<Article>();
        }

        protected override IQueryable<Article> WithIncludes(IQueryable<Article> query)
        {
            return query
                .Include(a => a.TypesOfArticle)
                    .ThenInclude(toa => toa.ArticleTypeNavigation)
                .Include(a => a.ArticleInterests)
                .Include(a => a.UserNavigation);
        }

        public async override Task<IEnumerable<Article>> GetByKeyAsync(string title)
        {
            return await WithIncludes(_articles.Where(a => a.Title.ToLower().Contains(title.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Article>> SearchAsync(
            string? title = null,
            string? content = null,
            int? userId = null,
            int? typeId = null,
            bool? isPremium = null)
        {
            var query = _articles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(title))
            {
                string titleLower = title.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(titleLower));
            }
            if (!string.IsNullOrWhiteSpace(content))
            {
                string contentLower = content.ToLower();
                query = query.Where(a => a.Content.ToLower().Contains(contentLower));
            }
            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);
            if (isPremium.HasValue)
                query = query.Where(a => a.IsPremium == isPremium.Value);
            if (typeId.HasValue)
                query = query.Where(a => a.TypesOfArticle.Any(toa => toa.ArticleTypeId == typeId.Value));
            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}