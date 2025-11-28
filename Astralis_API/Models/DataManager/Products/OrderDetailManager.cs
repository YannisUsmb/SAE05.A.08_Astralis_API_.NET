using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class OrderDetailManager : CrudManager<OrderDetail, int>, IOrderDetailRepository
    {
        public OrderDetailManager(AstralisDbContext context) : base(context)
        {
        }
    }
}