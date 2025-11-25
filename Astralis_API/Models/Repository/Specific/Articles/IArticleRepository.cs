using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IArticleRepository : IDataRepository<Article, int, string>
    {
    }
}
