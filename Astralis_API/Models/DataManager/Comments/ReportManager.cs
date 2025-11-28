using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ReportManager : CrudManager<Report, int>, IReportRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Report> _reports;

        public ReportManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _reports = _context.Set<Report>();
        }

        public async Task<IEnumerable<Report>> SearchAsync(
             int? statusId = null,
             int? motiveId = null,
             DateTime? minDate = null,
             DateTime? maxDate = null)
        {
            var query = _reports.AsQueryable();
            if (statusId.HasValue)
                query = query.Where(r => r.ReportStatusId == statusId.Value);
            if (motiveId.HasValue)
                query = query.Where(r => r.ReportMotiveId == motiveId.Value);
            if (minDate.HasValue)
                query = query.Where(r => r.Date >= minDate.Value);
            if (maxDate.HasValue)
                query = query.Where(r => r.Date <= maxDate.Value);
            query = query.OrderByDescending(r => r.Date);
            return await query
                .Include(r => r.AdminNavigation)
                .Include(r => r.CommentNavigation)
                .Include(r => r.ReportMotiveNavigation)
                .Include(r => r.ReportStatusNavigation)
                .Include(r => r.UserNavigation)
                .ToListAsync();
        }
    }
}