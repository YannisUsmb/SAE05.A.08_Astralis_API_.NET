using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IReportRepository : ICrudRepository<Report, int>
    {
        Task<IEnumerable<Report>> SearchAsync(
            int? statusId = null,
            int? motiveId = null,
            DateTime? minDate = null,
            DateTime? maxDate = null
        );
    }
}