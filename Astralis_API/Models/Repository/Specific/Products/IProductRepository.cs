using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IProductRepository : IDataRepository<Product, int, string>
    {
        Task<IEnumerable<Product>> SearchAsync(
                    string? searchText = null,
                    int? productCategoryId = null,
                    decimal? minPrice = null,
                    decimal? maxPrice = null
                );
    }
}