using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IReportRepository : ICrudRepository<Report, int>
    {
        Task<IEnumerable<Report>> GetByDateAsync(DateTime date);
    }
}