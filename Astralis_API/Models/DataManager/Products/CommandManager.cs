using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CommandManager : DataManager<Command, int, int>, ICommandRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Command> _command;

        public CommandManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _command = _context.Set<Command>();
        }

        protected override IQueryable<Command> WithIncludes(IQueryable<Command> query)
        {
            return query.Include(c => c.UserNavigation)
                .Include(c => c.CommandStatusNavigation)
                .Include(c => c.OrderDetails)
                    .ThenInclude(od => od.ProductNavigation);
        }

        public async override Task<IEnumerable<Command>> GetByKeyAsync(int id)
        {
            return await WithIncludes(_command.Where(c => c.UserId == id))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Command>> GetByCommandStatusIdAsync(int id)
        {
            return await WithIncludes(_command.Where(c => c.CommandStatusId == id))
                            .ToListAsync();
        }
    }
}
