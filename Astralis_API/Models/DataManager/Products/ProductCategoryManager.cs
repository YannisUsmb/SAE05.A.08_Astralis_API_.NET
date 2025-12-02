using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ProductCategoryManager : ReadableManager<ProductCategory, int>, IProductCategoryRepository
    {
        public ProductCategoryManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<ProductCategory?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(pc => pc.Id == id);
        }

        protected override IQueryable<ProductCategory> WithIncludes(IQueryable<ProductCategory> query)
        {
            return query.Include(pc => pc.Products);
        }
    }
}