using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ReportStatusManager : ReadableManager<ReportMotive, int>, IReportMotiveRepository
    {
        public ReportStatusManager(AstralisDbContext context) : base(context)
        {
        }
    }
}