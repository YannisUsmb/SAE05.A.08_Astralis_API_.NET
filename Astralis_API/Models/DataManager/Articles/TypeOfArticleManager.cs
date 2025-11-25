using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class TypeOfArticleManager : CrudManager<TypeOfArticleManager, int, string>
    {
        public TypeOfArticleManager(AstralisDbContext context) : base(context)
        {
        }        
    }
}