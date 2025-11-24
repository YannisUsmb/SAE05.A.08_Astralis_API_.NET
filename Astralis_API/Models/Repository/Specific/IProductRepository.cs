using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IProductRepository : IDataRepository<Product, int, string>
    {
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int id);
    }
}
