using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class EventManager : DataManager<Event, int, string>, IEventRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Event> _events;

        public EventManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _events = _context.Set<Event>();
        }

        protected override IQueryable<Event> WithIncludes(IQueryable<Event> query)
        {
            return query.Include(e => e.EventInterests)
                .Include(e => e.EventTypeNavigation)
                .Include(e => e.UserNavigation);
        }
        public async override Task<IEnumerable<Event>> GetByKeyAsync(string title)
        {
            return await WithIncludes(_events.Where(s => s.Title.ToLower().Contains(title.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetByStartDateAsync(DateTime date)
        {
            return await WithIncludes(_events.Where(s => s.StartDate == date))
                            .ToListAsync();
        }
    }
}