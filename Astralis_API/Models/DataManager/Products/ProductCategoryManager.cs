using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class ProductCategoryManager : ReadableManager<ProductCategory, int>, IProductCategoryRepository
    {
        public ProductCategoryManager(AstralisDbContext context) : base(context)
        {
        }
    }
}