using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IGalaxyQuasarRepository : IDataRepository<GalaxyQuasar, int, string>
    {
        Task<IEnumerable<GalaxyQuasar>> GetByGalaxyQuasarClassIdAsync(int id);
    }
}