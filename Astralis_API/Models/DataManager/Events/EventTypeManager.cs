using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class EventTypeManager : ReadableManager<EventType, int>, IEventTypeRepository
    {
        public EventTypeManager(AstralisDbContext context) : base(context)
        {
        }
        protected override IQueryable<EventType> WithIncludes(IQueryable<EventType> query)
        {
            return query.Include(et => et.Events);
        }
    }
}