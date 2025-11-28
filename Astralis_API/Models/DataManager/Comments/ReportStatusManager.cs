using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ReportStatusManager : ReadableManager<ReportStatus, int>, IReportStatusRepository
    {
        public ReportStatusManager(AstralisDbContext context) : base(context)
        {
        }

        protected override IQueryable<ReportStatus> WithIncludes(IQueryable<ReportStatus> query)
        {
            return query.Include(rs => rs.Reports);
        }
    }
}