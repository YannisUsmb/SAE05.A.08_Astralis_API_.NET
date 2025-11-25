using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class ReportMotiveManager : ReadableManager<ReportMotive, int>, IReportMotiveRepository
    {
        public ReportMotiveManager(AstralisDbContext context) : base(context)
        {
        }
    }
}