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
        protected override IQueryable<ReportMotive> WithIncludes(IQueryable<ReportMotive> query)
        {
            return query.Include(rm => rm.Reports);
        }
    }
}