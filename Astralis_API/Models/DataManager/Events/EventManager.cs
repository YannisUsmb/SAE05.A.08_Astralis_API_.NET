using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class EventManager : CrudManager<Event, int>, IEventRepository
    {
        public EventManager(AstralisDbContext context) : base(context)
        {
        }
    }
}