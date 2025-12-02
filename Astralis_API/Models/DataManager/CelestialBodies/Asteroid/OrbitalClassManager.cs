using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class OrbitalClassManager : ReadableManager<OrbitalClass, int>, IOrbitalClassRepository
    {
        public OrbitalClassManager(AstralisDbContext context) : base(context)
        {
        }
        public override async Task<OrbitalClass?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(oc => oc.Id == id);
        }

        protected override IQueryable<OrbitalClass> WithIncludes(IQueryable<OrbitalClass> query)
        {
            return query
                .Include(oc => oc.Asteroids);
        }
    }
}