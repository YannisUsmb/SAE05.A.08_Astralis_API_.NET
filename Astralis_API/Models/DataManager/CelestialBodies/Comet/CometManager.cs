using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CometManager : DataManager<Comet, int, string>, ICometRepository
    {
        public CometManager(AstralisDbContext context) : base(context)
        {
        }
        public override async Task<Comet?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(c => c.Id == id);
        }
        protected override IQueryable<Comet> WithIncludes(IQueryable<Comet> query)
        {
            return query.Include(c => c.CelestialBodyNavigation);
        }

        public async override Task<IEnumerable<Comet>> GetByKeyAsync(string reference)
        {
            return await WithIncludes(_entities.Where(c => c.Reference.ToLower().Contains(reference.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Comet>> SearchAsync(
            string? reference = null,
            decimal? minEccentricity = null,
            decimal? maxEccentricity = null,
            decimal? minInclination = null,
            decimal? maxInclination = null,
            decimal? minPerihelionAU = null,
            decimal? maxPerihelionAU = null,
            decimal? minAphelionAU = null,
            decimal? maxAphelionAU = null,
            decimal? minOrbitalPeriod = null,
            decimal? maxOrbitalPeriod = null,
            decimal? minMOID = null,
            decimal? maxMOID = null)
        {
            var query = _entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(reference))
            {
                string refLower = reference.ToLower();
                query = query.Where(c => c.Reference != null && c.Reference.ToLower().Contains(refLower));
            }
           
            if (minEccentricity.HasValue)
                query = query.Where(c => c.OrbitalEccentricity >= minEccentricity.Value);

            if (maxEccentricity.HasValue)
                query = query.Where(c => c.OrbitalEccentricity <= maxEccentricity.Value);

            if (minInclination.HasValue)
                query = query.Where(c => c.OrbitalInclinationDegrees >= minInclination.Value);

            if (maxInclination.HasValue)
                query = query.Where(c => c.OrbitalInclinationDegrees <= maxInclination.Value);

            if (minPerihelionAU.HasValue)
                query = query.Where(c => c.PerihelionDistanceAU >= minPerihelionAU.Value);

            if (maxPerihelionAU.HasValue)
                query = query.Where(c => c.PerihelionDistanceAU <= maxPerihelionAU.Value);

            if (minAphelionAU.HasValue)
                query = query.Where(c => c.AphelionDistanceAU >= minAphelionAU.Value);

            if (maxAphelionAU.HasValue)
                query = query.Where(c => c.AphelionDistanceAU <= maxAphelionAU.Value);

            if (minOrbitalPeriod.HasValue)
                query = query.Where(c => c.OrbitalPeriodYears >= minOrbitalPeriod.Value);

            if (maxOrbitalPeriod.HasValue)
                query = query.Where(c => c.OrbitalPeriodYears <= maxOrbitalPeriod.Value);

            if (minMOID.HasValue)
                query = query.Where(c => c.MinimumOrbitIntersectionDistanceAU >= minMOID.Value);

            if (maxMOID.HasValue)
                query = query.Where(c => c.MinimumOrbitIntersectionDistanceAU <= maxMOID.Value);

            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}