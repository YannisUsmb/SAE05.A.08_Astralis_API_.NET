using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IArticleRepository : IDataRepository<Article, int, string>
    {
        Task<IEnumerable<Article>> SearchAsync(
            string? title = null,
            string? content = null,
            int? userId = null,
            int? typeId = null,
            bool? isPremium = null
        );
    }
}
