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

        public override async Task<Article?> GetByIdAsync(int id)
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

        public async Task<(IEnumerable<Article> Items, int TotalCount)> SearchAsync(
            string? searchTerm,
            IEnumerable<int>? typeIds,
            bool? isPremium,
            string sortBy,
            int page,
            int pageSize)
        {
            var query = _entities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(term) || a.Content.ToLower().Contains(term));
            }

            if (isPremium.HasValue)
            {
                query = query.Where(a => a.IsPremium == isPremium.Value);
            }

            if (typeIds != null && typeIds.Any())
            {
                query = query.Where(a => a.TypesOfArticle.Any(toa => typeIds.Contains(toa.ArticleTypeId)));
            }

            query = sortBy switch
            {
                "date_asc" => query.OrderBy(a => a.PublicationDate),
                "title_asc" => query.OrderBy(a => a.Title),
                "title_desc" => query.OrderByDescending(a => a.Title),
                _ => query.OrderByDescending(a => a.PublicationDate)
            };

            int totalCount = await query.CountAsync();

            var items = await WithIncludes(query)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async override Task UpdateAsync(Article entityToUpdate, Article entity)
        {
            entityToUpdate.Title = entity.Title;
            entityToUpdate.Description = entity.Description;
            entityToUpdate.Content = entity.Content;
            entityToUpdate.CoverImageUrl = entity.CoverImageUrl;
            entityToUpdate.IsPremium = entity.IsPremium;

            await _context.Entry(entityToUpdate)
                .Collection(a => a.TypesOfArticle)
                .LoadAsync();

            entityToUpdate.TypesOfArticle.Clear();

            if (entity.TypesOfArticle != null)
            {
                foreach (var newItem in entity.TypesOfArticle)
                {
                    entityToUpdate.TypesOfArticle.Add(new TypeOfArticle
                    {
                        ArticleId = entityToUpdate.Id,
                        ArticleTypeId = newItem.ArticleTypeId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}