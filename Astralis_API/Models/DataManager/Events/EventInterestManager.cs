using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class EventInterestManager : JoinManager<EventInterest, int, int>, IEventInterestRepository
    {
        public EventInterestManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<EventInterest?> GetByIdAsync(int eventId, int userId)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(ei => ei.EventId == eventId && ei.UserId == userId);
        }

        protected override IQueryable<EventInterest> WithIncludes(IQueryable<EventInterest> query)
        {
            return query.Include(ei => ei.UserNavigation)
                        .Include(ei => ei.EventNavigation);
        }
    }
}