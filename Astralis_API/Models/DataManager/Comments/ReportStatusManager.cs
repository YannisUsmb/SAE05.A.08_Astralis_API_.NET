using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class ReportStatusManager : ReadableManager<ReportStatus, int>, IReportStatusRepository
    {
        public ReportStatusManager(AstralisDbContext context) : base(context)
        {
        }
    }
}