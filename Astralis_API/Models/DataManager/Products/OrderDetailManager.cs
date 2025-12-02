using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class OrderDetailManager : JoinManager<OrderDetail, int, int>, IOrderDetailRepository
    {
        public OrderDetailManager(AstralisDbContext context) : base(context)
        {
        }

        protected override IQueryable<OrderDetail> WithIncludes(IQueryable<OrderDetail> query)
        {
            return query.Include(od => od.ProductNavigation)
                .Include(od => od.CommandNavigation);
        }
    }
}