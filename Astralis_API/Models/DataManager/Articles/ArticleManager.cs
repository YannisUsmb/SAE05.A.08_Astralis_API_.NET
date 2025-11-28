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

        public async override Task<IEnumerable<Article>> GetByKeyAsync(string title)
        {
            return await _articles.Where(a => a.Title.ToLower().Contains(title.ToLower()))
                            .Include(a => a.TypesOfArticle)
                            .Include(a => a.ArticleInterests)
                            .Include(a => a.UserNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetByTypeOfArticle(int idtype)
        {
            return await _articles.Where(a => a.TypesOfArticle.Any(toa => toa.ArticleTypeId == idtype))
                .Include(a => a.TypesOfArticle)
                .ThenInclude(toa => toa.ArticleTypeNavigation)
                .Include(a => a.ArticleInterests)
                .Include(a => a.UserNavigation)
                .ToListAsync();
        }
    }
}