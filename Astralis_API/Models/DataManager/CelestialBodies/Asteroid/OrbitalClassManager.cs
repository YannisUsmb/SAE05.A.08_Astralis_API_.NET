using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class OrbitalClassManager : CrudManager<OrbitalClass, int>, IOrbitalClassRepository
    {
        public OrbitalClassManager(AstralisDbContext context) : base(context)
        {
        }

        protected override IQueryable<OrbitalClass> WithIncludes(IQueryable<OrbitalClass> query)
        {
            return query
                .Include(oc => oc.Asteroids);
        }
    }
}