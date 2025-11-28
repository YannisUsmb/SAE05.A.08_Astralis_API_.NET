using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class DiscoveryStatusManager : CrudManager<DiscoveryStatus, int>, IDiscoveryStatusRepository
    {
        public DiscoveryStatusManager(AstralisDbContext context) : base(context)
        {
        }
        protected override IQueryable<DiscoveryStatus> WithIncludes(IQueryable<DiscoveryStatus> query)
        {
            return query.Include(d => d.Discoveries);
        }
    }
}