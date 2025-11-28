using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IDiscoveryRepository : IDataRepository<Discovery, int, string>
    {
        Task<IEnumerable<Discovery>> SearchAsync(
            string? title = null,
            int? userId = null,
            int? celestialBodyId = null,
            int? discoveryStatusId = null,
            int? aliasStatusId = null,
            int? discoveryApprovalUserId = null,
            int? aliasApprovalUserId = null
        );
    }
}