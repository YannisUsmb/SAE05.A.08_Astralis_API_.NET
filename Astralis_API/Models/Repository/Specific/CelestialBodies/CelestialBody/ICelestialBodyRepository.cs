using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface ICelestialBodyRepository : IDataRepository<CelestialBody, int, string>
    {
        Task<IDictionary<int, string>> GetSubtypesByMainTypeAsync(int mainTypeId);
        
        Task<IEnumerable<CelestialBody>> SearchAsync(
            string? searchText = null,
            IEnumerable<int>? celestialBodyTypeIds = null,
            bool? isDiscovery = null,
            int? subtypeId = null,
            int pageNumber = 1,
            int pageSize = 30
        );
    }
}