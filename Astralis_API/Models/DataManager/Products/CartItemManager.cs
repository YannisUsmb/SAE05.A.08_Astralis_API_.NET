using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CartItemManager : JoinManager<CartItem, int, int>, ICartItemRepository
    {
        public CartItemManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<CartItem?> GetByIdAsync(int userId, int productId)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        protected override IQueryable<CartItem> WithIncludes(IQueryable<CartItem> query)
        {
            return query.Include(ci => ci.ProductNavigation)
                .Include(ci => ci.UserNavigation);
        }
    }
}