using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface ICartItemRepository : IJoinRepository<CartItem, int, int>
    {
        Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId);
        Task ClearCartAsync(int userId);
    }
}
