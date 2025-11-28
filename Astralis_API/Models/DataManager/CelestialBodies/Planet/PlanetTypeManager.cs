using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class PlanetTypeManager : CrudManager<PlanetType, int>, IPlanetTypeRepository
    {
        public PlanetTypeManager(AstralisDbContext context) : base(context)
        {
        }
        protected override IQueryable<PlanetType> WithIncludes(IQueryable<PlanetType> query)
        {
            return query.Include(pt => pt.Planets);
        }
    }
}