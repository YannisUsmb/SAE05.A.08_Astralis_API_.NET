using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CelestialBodyTypeManager : ReadableManager<CelestialBodyType, int>, ICelestialBodyTypeRepository
    {
        public CelestialBodyTypeManager(AstralisDbContext context) : base(context)
        {
        }
    }
}