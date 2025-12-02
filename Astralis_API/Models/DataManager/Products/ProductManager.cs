using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ProductManager : DataManager<Product, int, string>, IProductRepository
    {
        public ProductManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(p => p.Id == id);
        }

        protected override IQueryable<Product> WithIncludes(IQueryable<Product> query)
        {
            return query
                .Include(p => p.ProductCategoryNavigation)
                .Include(p => p.UserNavigation)
                .Include(p => p.CartItems)
                .Include(p => p.OrderDetails);
        }

        public async override Task<IEnumerable<Product>> GetByKeyAsync(string name)
        {
            return await WithIncludes(_entities.Where(p => p.Label.ToLower().Contains(name.ToLower()))).ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(
            string? searchText = null,
            IEnumerable<int>? productCategoryIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var query = _entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string textLower = searchText.ToLower();
                query = query.Where(p =>
                    p.Label.ToLower().Contains(textLower)
                    || (p.Description != null && p.Description.ToLower().Contains(textLower))
                );
            }

            if (productCategoryIds != null && productCategoryIds.Any())           
                query = query.Where(p => productCategoryIds.Contains(p.ProductCategoryId));
            
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}