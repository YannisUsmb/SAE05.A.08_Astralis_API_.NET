using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class PlanetTypeManager : ReadableManager<PlanetType, int>, IPlanetTypeRepository
    {
        public PlanetTypeManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<PlanetType?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        protected override IQueryable<PlanetType> WithIncludes(IQueryable<PlanetType> query)
        {
            return query.Include(pt => pt.Planets);
        }
    }
}