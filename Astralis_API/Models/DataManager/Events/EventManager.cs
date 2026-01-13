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

        public async Task<(IEnumerable<Event> Items, int TotalCount)> SearchAsync(
            string? searchText,
            IEnumerable<int>? eventTypeIds,
            DateTime? minStartDate,
            DateTime? maxStartDate,
            DateTime? minEndDate,
            DateTime? maxEndDate,
            int pageNumber,
            int pageSize,
            string sortBy)
        {
            var query = _entities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e => e.Title.Contains(searchText) || (e.Description != null && e.Description.Contains(searchText)));
            }

            if (eventTypeIds != null && eventTypeIds.Any())
            {
                query = query.Where(e => eventTypeIds.Contains(e.EventTypeId));
            }

            if (minStartDate.HasValue) query = query.Where(e => e.StartDate >= minStartDate.Value);
            if (maxStartDate.HasValue) query = query.Where(e => e.StartDate <= maxStartDate.Value);
            if (minEndDate.HasValue) query = query.Where(e => e.EndDate >= minEndDate.Value);
            if (maxEndDate.HasValue) query = query.Where(e => e.EndDate <= maxEndDate.Value);

            query = sortBy switch
            {
                "date_desc" => query.OrderByDescending(e => e.StartDate),
                "alpha_asc" => query.OrderBy(e => e.Title),
                "alpha_desc" => query.OrderByDescending(e => e.Title),
                "type" => query.OrderBy(e => e.EventTypeNavigation.Label),
                "date_asc" or _ => query.OrderBy(e => e.StartDate)
            };

            int totalCount = await query.CountAsync();

            var items = await WithIncludes(query)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}