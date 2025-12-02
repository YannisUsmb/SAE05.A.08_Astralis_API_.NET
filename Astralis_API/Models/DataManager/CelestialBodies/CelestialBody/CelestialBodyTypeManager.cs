using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CelestialBodyTypeManager : ReadableManager<CelestialBodyType, int>, ICelestialBodyTypeRepository
    {
        public CelestialBodyTypeManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<CelestialBodyType?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(cbt => cbt.Id == id);
        }

        protected override IQueryable<CelestialBodyType> WithIncludes(IQueryable<CelestialBodyType> query)
        {
            return query
                    .Include(cbt => cbt.CelestialBodies)
                    .Include(cbt => cbt.Audios);
        }
    }
}