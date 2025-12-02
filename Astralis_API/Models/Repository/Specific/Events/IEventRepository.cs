using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IEventRepository : IDataRepository<Event, int, string>
    {
        Task<IEnumerable<Event>> SearchAsync(
            string? searchText = null,
            IEnumerable<int>? eventTypeIds = null,
            DateTime? minStartDate = null,
            DateTime? maxStartDate = null,
            DateTime? minEndDate = null,
            DateTime? maxEndDate = null
        );
    }
}