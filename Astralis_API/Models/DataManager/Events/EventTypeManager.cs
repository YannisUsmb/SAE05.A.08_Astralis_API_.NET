using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class EventTypeManager : CrudManager<EventType, int>, IEventTypeRepository
    {
        public EventTypeManager(AstralisDbContext context) : base(context)
        {
        }
    }
}