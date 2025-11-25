using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class OrbitalClassManager : CrudManager<OrbitalClass, int, string>
    {
        public OrbitalClassManager(AstralisDbContext context) : base(context)
        {
        }

    }
}