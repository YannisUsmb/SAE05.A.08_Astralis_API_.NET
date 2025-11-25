using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CartItemManager : CrudManager<CartItem, int>, ICartItemRepository
    {
        public CartItemManager(AstralisDbContext context) : base(context)
        {
        }
    }
}