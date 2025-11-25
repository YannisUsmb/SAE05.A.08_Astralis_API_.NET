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

        public async override Task<IEnumerable<Event>> GetByKeyAsync(string title)
        {
            return await _events.Where(s => s.Title.ToLower().Contains(title.ToLower()))
                            .Include(s => s.EventTypeNavigation)
                            .Include(s => s.UserNavigation)
                            .Include(s => s.EventInterests)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetByStartDateAsync(DateTime date)
        {
            return await _events.Where(s => s.StartDate == date)
                            .Include(s => s.EventTypeNavigation)
                            .Include(s => s.UserNavigation)
                            .Include(s => s.EventInterests)
                            .ToListAsync();
        }
    }
}