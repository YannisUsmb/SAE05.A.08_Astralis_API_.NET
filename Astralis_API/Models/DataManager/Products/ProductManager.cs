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

        public async override Task<IEnumerable<Product>> GetByKeyAsync(string name)
        {
            return await _products.Where(p => p.Label.ToLower().Contains(name.ToLower()))
                            .Include(p => p.ProductCategoryNavigation)
                            .Include(p => p.UserNavigation)
                            .Include(p => p.CartItems)
                            .Include(p => p.OrderDetails)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByProductCategoryIdAsync(int id)
        {
            return await _products.Where(p => p.ProductCategoryId == id)
                            .Include(p => p.ProductCategoryNavigation)
                            .Include(p => p.UserNavigation)
                            .Include(p => p.CartItems)
                            .Include(p => p.OrderDetails)
                            .ToListAsync();
        }
    }
}
