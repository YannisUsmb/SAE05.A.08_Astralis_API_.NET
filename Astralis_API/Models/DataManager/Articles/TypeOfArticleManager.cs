using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class TypeOfArticleManager : CrudManager<TypeOfArticle, int>, ITypeOfArticleRepository
    {
        public TypeOfArticleManager(AstralisDbContext context) : base(context)
        {
        }        
    }
}