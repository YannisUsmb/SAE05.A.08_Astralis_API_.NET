using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CommandStatusManager : ReadableManager<CommandStatus, int>, ICommandStatusRepository
    {
        public CommandStatusManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<CommandStatus?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        protected override IQueryable<CommandStatus> WithIncludes(IQueryable<CommandStatus> query)
        {
            return query.Include(cs => cs.Commands);
        }
    }
}