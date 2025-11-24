using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ArticleTypeManager : DataManager<Article, int, string>, ISearchRepository<Article, string>
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Article> _articles;

        public ArticleTypeManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _articles = _context.Set<Article>();
        }

        public async Task<IEnumerable<Article>> GetByKeyAsync(string title)
        {
            return await _articles.Where(cb => cb.Title.ToLower().Contains(title.ToLower()))
                            .Include(cb => cb.TypesOfArticle)
                            .Include(cb=> cb.ArticleInterests)
                            .Include(cb=> cb.UserNavigation)
                            .ToListAsync();
        }
    }
}