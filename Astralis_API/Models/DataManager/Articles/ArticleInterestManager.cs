using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class ArticleInterestInterestManager : DataManager<ArticleInterest, int, string>
    {

        public ArticleInterestInterestManager(AstralisDbContext context) : base(context)
        {
        }
    }
}