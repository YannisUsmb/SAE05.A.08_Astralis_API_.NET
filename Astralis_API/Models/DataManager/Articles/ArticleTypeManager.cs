using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class ArticleTypeTypeManager : DataManager<ArticleType, int, string>
    {
        public ArticleTypeTypeManager(AstralisDbContext context) : base(context)
        {
        }        
    }
}