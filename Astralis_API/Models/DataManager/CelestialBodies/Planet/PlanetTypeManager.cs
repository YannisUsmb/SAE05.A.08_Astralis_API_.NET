using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class PlanetTypeManager : CrudManager<PlanetType, int>, IPlanetTypeRepository
    {
        public PlanetTypeManager(AstralisDbContext context) : base(context)
        {
        }
    }
}