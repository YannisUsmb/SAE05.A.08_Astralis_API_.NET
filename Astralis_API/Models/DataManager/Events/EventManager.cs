using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class EventManager : DataManager<Event, int, string>, IEventRepository
    {
        public EventManager(AstralisDbContext context) : base(context)
        {
        }
        public override async Task<Event?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(e => e.Id == id);
        }

        protected override IQueryable<Event> WithIncludes(IQueryable<Event> query)
        {
            return query
                .Include(e => e.EventInterests)
                    .ThenInclude(ei => ei.UserNavigation)
                .Include(e => e.EventTypeNavigation)
                .Include(e => e.UserNavigation);
        }

        public async override Task<IEnumerable<Event>> GetByKeyAsync(string title)
        {
            return await WithIncludes(_entities.Where(s => s.Title.ToLower().Contains(title.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Event>> SearchAsync(
            string? searchText = null,
            IEnumerable<int>? eventTypeIds = null,
            DateTime? minStartDate = null,
            DateTime? maxStartDate = null,
            DateTime? minEndDate = null,
            DateTime? maxEndDate = null)
        {
            var query = _entities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string textLower = searchText.ToLower();
                query = query.Where(e =>
                    e.Title.ToLower().Contains(textLower)
                    || e.Description.ToLower().Contains(textLower)
                    || (e.Location != null && e.Location.ToLower().Contains(textLower))
                );
            }
            if (eventTypeIds != null && eventTypeIds.Any())
            {
                query = query.Where(e => eventTypeIds.Contains(e.EventTypeId));
            }

            if (minStartDate.HasValue)
                query = query.Where(e => e.StartDate >= minStartDate.Value);

            if (maxStartDate.HasValue)
                query = query.Where(e => e.StartDate <= maxStartDate.Value);

            if (minEndDate.HasValue)
                query = query.Where(e => e.EndDate >= minEndDate.Value);

            if (maxEndDate.HasValue)
                query = query.Where(e => e.EndDate <= maxEndDate.Value);

            return await WithIncludes(query).ToListAsync();
        }
    }
}