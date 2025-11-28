using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CartItemManager : CrudManager<CartItem, int>, ICartItemRepository
    {
        public CartItemManager(AstralisDbContext context) : base(context)
        {
        }

        protected override IQueryable<CartItem> WithIncludes(IQueryable<CartItem> query)
        {
            return query.Include(ci => ci.ProductNavigation)
                .Include(ci => ci.UserNavigation);
        }
    }
}