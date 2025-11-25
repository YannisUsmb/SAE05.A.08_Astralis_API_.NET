using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class ReportManager : CrudManager<Report, int>, IReportRepository
    {
        public ReportManager(AstralisDbContext context) : base(context)
        {
        }
    }
}