using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class GalaxyQuasarClassManager : CrudManager<GalaxyQuasarClass, int>, IGalaxyQuasarClassRepository
    {
        public GalaxyQuasarClassManager(AstralisDbContext context) : base(context)
        {
        }
    }
}