using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class EventInterestManager : CrudManager<EventInterest, int>, IEventInterestRepository
    {
        public EventInterestManager(AstralisDbContext context) : base(context)
        {
        }
    }
}