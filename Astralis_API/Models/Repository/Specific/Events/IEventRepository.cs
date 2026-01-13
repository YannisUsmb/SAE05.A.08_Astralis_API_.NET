using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IEventRepository : IDataRepository<Event, int, string>
    {
        Task<(IEnumerable<Event> Items, int TotalCount)> SearchAsync(
            string? searchText,
            IEnumerable<int>? eventTypeIds,
            DateTime? minStartDate,
            DateTime? maxStartDate,
            DateTime? minEndDate,
            DateTime? maxEndDate,
            int pageNumber,
            int pageSize,
            string sortBy);
    }
}