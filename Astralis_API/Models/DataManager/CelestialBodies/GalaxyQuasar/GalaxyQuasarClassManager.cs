using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class GalaxyQuasarClassManager : CrudManager<GalaxyQuasarClass, int>, IGalaxyQuasarClassRepository
    {
        public GalaxyQuasarClassManager(AstralisDbContext context) : base(context)
        {
        }
        protected override IQueryable<GalaxyQuasarClass> WithIncludes(IQueryable<GalaxyQuasarClass> query)
        {
            return query.Include(d => d.GalaxiesQuasars);
        }
    }
}