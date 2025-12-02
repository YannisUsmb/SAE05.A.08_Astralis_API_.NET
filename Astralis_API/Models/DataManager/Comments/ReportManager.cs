using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ReportManager : CrudManager<Report, int>, IReportRepository
    {
        public ReportManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<Report?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(r => r.Id == id);
        }

        protected override IQueryable<Report> WithIncludes(IQueryable<Report> query)
        {
            return query.Include(r => r.AdminNavigation)
                .Include(r => r.CommentNavigation)
                .Include(r => r.ReportMotiveNavigation)
                .Include(r => r.ReportStatusNavigation)
                .Include(r => r.UserNavigation);
        }

        public async Task<IEnumerable<Report>> SearchAsync(
             int? statusId = null,
             int? motiveId = null,
             DateTime? minDate = null,
             DateTime? maxDate = null)
        {
            var query = _entities.AsQueryable();
            if (statusId.HasValue)
                query = query.Where(r => r.ReportStatusId == statusId.Value);
            if (motiveId.HasValue)
                query = query.Where(r => r.ReportMotiveId == motiveId.Value);
            if (minDate.HasValue)
                query = query.Where(r => r.Date >= minDate.Value);
            if (maxDate.HasValue)
                query = query.Where(r => r.Date <= maxDate.Value);
            query = query.OrderByDescending(r => r.Date);
            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}