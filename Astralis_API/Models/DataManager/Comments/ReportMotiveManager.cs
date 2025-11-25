using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ReportMotiveManager : CrudManager<ReportMotive, int>, IReportMotiveRepository
    {
        public ReportMotiveManager(AstralisDbContext context) : base(context)
        {
        }
    }
}