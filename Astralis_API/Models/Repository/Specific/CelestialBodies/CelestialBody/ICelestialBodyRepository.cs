using Astralis_API.Models.EntityFramework;
using Astralis.Shared.DTOs;

namespace Astralis_API.Models.Repository
{
    public interface ICelestialBodyRepository : IDataRepository<CelestialBody, int, string>
    {
        Task<List<CelestialBodySubtypeDto>> GetSubtypesByMainTypeAsync(int mainTypeId);
        Task<IEnumerable<DetectionMethod>> GetDetectionMethodsAsync();
        
        Task<IEnumerable<CelestialBody>> SearchAsync(
            CelestialBodyFilterDto filter,
            int pageNumber = 1,
            int pageSize = 30
        );
    }
}