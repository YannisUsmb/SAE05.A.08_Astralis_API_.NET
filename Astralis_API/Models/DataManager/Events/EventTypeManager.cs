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

        public override async Task<EventType?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(et => et.Id == id);
        }

        protected override IQueryable<EventType> WithIncludes(IQueryable<EventType> query)
        {
            return query.Include(et => et.Events);
        }
    }
}