using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ReportMotiveManager : ReadableManager<ReportMotive, int>, IReportMotiveRepository
    {
        public ReportMotiveManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<ReportMotive?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(rm => rm.Id == id);
        }

        protected override IQueryable<ReportMotive> WithIncludes(IQueryable<ReportMotive> query)
        {
            return query.Include(rm => rm.Reports);
        }
    }
}