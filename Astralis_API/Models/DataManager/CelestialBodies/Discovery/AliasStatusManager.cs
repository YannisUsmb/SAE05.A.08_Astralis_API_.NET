using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AliasStatusManager : CrudManager<AliasStatus, int>, IAliasStatusRepository
    {
        public AliasStatusManager(AstralisDbContext context) : base(context)
        {
        }
        protected override IQueryable<AliasStatus> WithIncludes(IQueryable<AliasStatus> query)
        {
            return query.Include(als => als.Discoveries);
        }
    }
}