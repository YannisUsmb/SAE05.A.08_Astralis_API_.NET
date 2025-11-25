using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IPlanetRepository : IDataRepository<Planet, int, string>
    {
        Task<IEnumerable<Planet>> GetByPlanetTypeIdAsync(int id);
    }
}