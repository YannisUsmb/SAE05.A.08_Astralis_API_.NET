using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IEventRepository : IDataRepository<Event, int, string>
    {
        Task<IEnumerable<Event>> GetByStartDateAsync(DateTime id);
    }
}
