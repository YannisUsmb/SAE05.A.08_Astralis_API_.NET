using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IEventRepository : ICrudRepository<Event, int>
    {
    }
}