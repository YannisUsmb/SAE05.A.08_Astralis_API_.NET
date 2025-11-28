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

        protected override IQueryable<CelestialBodyType> WithIncludes(IQueryable<CelestialBodyType> query)
        {
            return query.Include(cbt => cbt.CelestialBodies);
        }
    }
}