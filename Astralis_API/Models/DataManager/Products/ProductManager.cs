using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ProductManager : DataManager<Product, int, string>, IProductRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Product> _products;

        public ProductManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _products = _context.Set<Product>();
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
            return await WithIncludes(_products.Where(p => p.Label.ToLower().Contains(name.ToLower()))).ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(
            string? searchText = null,
            int? productCategoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var query = _products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string textLower = searchText.ToLower();
                query = query.Where(p =>
                    p.Label.ToLower().Contains(textLower)
                    || (p.Description != null && p.Description.ToLower().Contains(textLower))
                );
            }
            if (productCategoryId.HasValue)
                query = query.Where(p => p.ProductCategoryId == productCategoryId.Value);
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);
            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}