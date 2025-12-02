using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AsteroidManager : DataManager<Asteroid, int, string>, IAsteroidRepository
    {
        public AsteroidManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<Asteroid?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(a => a.Id == id);
        }

        protected override IQueryable<Asteroid> WithIncludes(IQueryable<Asteroid> query)
        {
            return query
                .Include(a => a.CelestialBodyNavigation)
                .Include(a => a.OrbitalClassNavigation);
        }

        public async override Task<IEnumerable<Asteroid>> GetByKeyAsync(string reference)
        {
            return await WithIncludes(_entities.Where(a => a.Reference.ToLower().Contains(reference.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Asteroid>> SearchAsync(
            string? reference = null,
            IEnumerable<int>? orbitalClassIds = null, 
            bool? isPotentiallyHazardous = null,
            int? orbitId = null,
            decimal? minAbsoluteMagnitude = null,
            decimal? maxAbsoluteMagnitude = null, 
            decimal? minDiameter = null,
            decimal? maxDiameter = null,
            decimal? minInclination = null,
            decimal? maxInclination = null,
            decimal? minSemiMajorAxis = null,
            decimal? maxSemiMajorAxis = null)
        {
            var query = _entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(reference))
                query = query.Where(a => a.Reference != null && a.Reference.ToLower().Contains(reference.ToLower()));
            
            if (orbitalClassIds != null && orbitalClassIds.Any())
            {
                query = query.Where(a => orbitalClassIds.Contains(a.OrbitalClassId));
            }

            if (orbitId.HasValue)
                query = query.Where(a => a.OrbitId == orbitId.Value);

            if (isPotentiallyHazardous.HasValue)
                query = query.Where(a => a.IsPotentiallyHazardous == isPotentiallyHazardous.Value);

            if (minAbsoluteMagnitude.HasValue)
                query = query.Where(a => a.AbsoluteMagnitude >= minAbsoluteMagnitude.Value);

            if (maxAbsoluteMagnitude.HasValue)
                query = query.Where(a => a.AbsoluteMagnitude <= maxAbsoluteMagnitude.Value);

            if (minDiameter.HasValue)
                query = query.Where(a => a.DiameterMinKm >= minDiameter.Value);

            if (maxDiameter.HasValue)
                query = query.Where(a => a.DiameterMaxKm <= maxDiameter.Value);

            if (minInclination.HasValue)
                query = query.Where(a => a.Inclination >= minInclination.Value);

            if (maxInclination.HasValue)
                query = query.Where(a => a.Inclination <= maxInclination.Value);
                        
            if (minSemiMajorAxis.HasValue)
                query = query.Where(a => a.SemiMajorAxis >= minSemiMajorAxis.Value);

            if (maxSemiMajorAxis.HasValue)
                query = query.Where(a => a.SemiMajorAxis <= maxSemiMajorAxis.Value);           

            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}