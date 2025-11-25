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

        public async Task<IEnumerable<Report>> GetByDateAsync(DateTime date)
        {
            return await _reports.Where(s => s.Date == date)
                            .Include(s => s.AdminNavigation)
                            .Include(s => s.CommentNavigation)
                            .Include(s => s.ReportMotiveNavigation)
                            .Include(s => s.ReportStatusNavigation)
                            .Include(s => s.UserNavigation)
                            .ToListAsync();
        }
    }
}