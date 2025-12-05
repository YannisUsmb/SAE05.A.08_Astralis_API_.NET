using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IOrderDetailRepository : IJoinRepository<OrderDetail, int, int>
    {
        Task AddRangeAsync(IEnumerable<OrderDetail> orderDetails);
        Task<IEnumerable<OrderDetail>> GetByCommandIdAsync(int commandId);
    }
}