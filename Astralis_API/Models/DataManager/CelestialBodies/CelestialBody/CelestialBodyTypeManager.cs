using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class CelestialBodyTypeManager : ReadableManager<CelestialBodyType, int>
    {

        public CelestialBodyTypeManager(AstralisDbContext context) : base(context)
        {

        }
    }
}