using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class OrbitalClassManager : CrudManager<OrbitalClass, int>, IOrbitalClassRepository
    {
        public OrbitalClassManager(AstralisDbContext context) : base(context)
        {
        }

    }
}