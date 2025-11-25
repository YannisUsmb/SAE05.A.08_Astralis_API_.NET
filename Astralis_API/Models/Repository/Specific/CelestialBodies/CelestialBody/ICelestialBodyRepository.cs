using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface ICelestialBodyRepository : IDataRepository<CelestialBody, int, string>
    {
        Task<IEnumerable<CelestialBody>> GetByCelestialBodyTypeIdAsync(int id);
    }
}