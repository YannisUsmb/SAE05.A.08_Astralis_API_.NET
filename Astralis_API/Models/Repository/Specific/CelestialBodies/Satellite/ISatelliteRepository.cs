using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface ISatelliteRepository : IDataRepository<Satellite, int, string>
    {
        Task<IEnumerable<Satellite>> GetByPlanetIdAsync(int id);
    }
}