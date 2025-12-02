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

        protected override IQueryable<EventInterest> WithIncludes(IQueryable<EventInterest> query)
        {
            return query.Include(ei => ei.UserNavigation)
                        .Include(ei => ei.EventNavigation);
        }
    }
}