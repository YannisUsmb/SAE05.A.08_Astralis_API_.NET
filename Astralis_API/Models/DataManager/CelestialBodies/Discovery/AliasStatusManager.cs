using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AliasStatusManager : ReadableManager<AliasStatus, int>, IAliasStatusRepository
    {
        public AliasStatusManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<AliasStatus?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(als => als.Id == id);
        }

        protected override IQueryable<AliasStatus> WithIncludes(IQueryable<AliasStatus> query)
        {
            return query.Include(als => als.Discoveries);
        }
    }
}