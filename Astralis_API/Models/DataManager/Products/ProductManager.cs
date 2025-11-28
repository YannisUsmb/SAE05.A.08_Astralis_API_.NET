using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
            return await WithIncludes(_products.Where(p => p.Label.ToLower().Contains(name.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByProductCategoryIdAsync(int id)
        {
            return await WithIncludes(_products.Where(p => p.ProductCategoryId == id))                            
                            .ToListAsync();
        }
    }
}
