using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class DiscoveryStatusManager : ReadableManager<DiscoveryStatus, int>, IDiscoveryStatusRepository
    {
        public DiscoveryStatusManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<DiscoveryStatus?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(ds => ds.Id == id);
        }

        protected override IQueryable<DiscoveryStatus> WithIncludes(IQueryable<DiscoveryStatus> query)
        {
            return query.Include(d => d.Discoveries);
        }
    }
}