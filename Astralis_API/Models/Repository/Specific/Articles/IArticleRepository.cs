using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IArticleRepository : IDataRepository<Article, int, string>
    {
        Task<IEnumerable<Article>> SearchAsync(
            string? searchTerm = null,
            int? userId = null,
            int? typeId = null,
            bool? isPremium = null
        );
    }
}
