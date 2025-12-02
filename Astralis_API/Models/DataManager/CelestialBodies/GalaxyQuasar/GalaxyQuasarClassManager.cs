using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class GalaxyQuasarClassManager : ReadableManager<GalaxyQuasarClass, int>, IGalaxyQuasarClassRepository
    {
        public GalaxyQuasarClassManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<GalaxyQuasarClass?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(gqc => gqc.Id == id);
        }

        protected override IQueryable<GalaxyQuasarClass> WithIncludes(IQueryable<GalaxyQuasarClass> query)
        {
            return query.Include(gqc => gqc.GalaxiesQuasars);
        }
    }
}