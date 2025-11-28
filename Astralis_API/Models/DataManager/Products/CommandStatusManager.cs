using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CommandStatusManager : ReadableManager<CommandStatus, int>, ICommandStatusRepository
    {
        public CommandStatusManager(AstralisDbContext context) : base(context)
        {
        }
    }
}