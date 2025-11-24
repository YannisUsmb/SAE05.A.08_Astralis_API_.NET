using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ArticleTypeTypeManager : DataManager<ArticleType, int, string>
    {
        public ArticleTypeTypeManager(AstralisDbContext context) : base(context)
        {
        }        
    }
}