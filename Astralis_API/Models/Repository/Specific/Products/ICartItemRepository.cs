using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface ICartItemRepository : IJoinRepository<CartItem, int, int>
    {
    }
}
