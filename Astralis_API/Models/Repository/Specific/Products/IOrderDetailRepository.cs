using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IOrderDetailRepository : IJoinRepository<OrderDetail, int, int>
    {
    }
}