using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IAsteroidRepository : IDataRepository<Asteroid, int, string>
    {
    }
}
