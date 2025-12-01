using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IArticleInterestRepository : IJoinRepository<ArticleInterest, int, int>
    {
        Task<IEnumerable<ArticleInterest>> GetByUserIdAsync(int userId);
    }
}