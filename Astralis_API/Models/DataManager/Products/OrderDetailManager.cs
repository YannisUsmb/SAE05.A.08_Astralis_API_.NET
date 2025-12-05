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

        public override async Task<OrderDetail?> GetByIdAsync(int commandId, int productId)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(od => od.CommandId == commandId && od.ProductId == productId);
        }

        public async Task AddRangeAsync(IEnumerable<OrderDetail> orderDetails)
        {
            await _entities.AddRangeAsync(orderDetails);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetByCommandIdAsync(int commandId)
        {
            return await WithIncludes(_entities)
                         .Where(od => od.CommandId == commandId)
                         .ToListAsync();
        }

        protected override IQueryable<OrderDetail> WithIncludes(IQueryable<OrderDetail> query)
        {
            return query.Include(od => od.ProductNavigation)
                .Include(od => od.CommandNavigation);
        }
    }
}