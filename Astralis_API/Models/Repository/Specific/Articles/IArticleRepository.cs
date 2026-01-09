using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IArticleRepository : IDataRepository<Article, int, string>
    {
        Task<(IEnumerable<Article> Items, int TotalCount)> SearchAsync(
            string? searchTerm,
            IEnumerable<int>? typeIds,
            bool? isPremium,
            string sortBy,
            int page,
            int pageSize);
    }
}
