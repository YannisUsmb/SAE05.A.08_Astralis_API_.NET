using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class DiscoveryStatusManager : CrudManager<DiscoveryStatus, int>, IDiscoveryStatusRepository
    {
        public DiscoveryStatusManager(AstralisDbContext context) : base(context)
        {
        }
    }
}