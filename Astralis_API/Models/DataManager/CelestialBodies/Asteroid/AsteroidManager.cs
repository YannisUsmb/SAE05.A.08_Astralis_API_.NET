using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AsteroidManager : DataManager<Asteroid, int, string>, IAsteroidRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Asteroid> _asteroids;

        public AsteroidManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _asteroids = _context.Set<Asteroid>();
        }

        public async override Task<IEnumerable<Asteroid>> GetByKeyAsync(string reference)
        {
            return await _asteroids.Where(cb => cb.Reference.ToLower().Contains(reference.ToLower()))
                            .Include(cb => cb.CelestialBodyNavigation)
                            .Include(cb => cb.OrbitalClassNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Asteroid>> GetByOrbitalIClassIdAsync(int id)
        {
            return await _asteroids.Where(cb => cb.OrbitalClassId == id)
                            .Include(cb => cb.CelestialBodyNavigation)
                            .Include(cb => cb.OrbitalClassNavigation)
                            .ToListAsync();
        }

        /*
            Exemple d'utilisation :
                var results = await _manager.SearchAsync(
                    minDiameter: 5.0,      // Plus grand que 5km
                     maxInclination: 10.0   // Plus petit que 10 degrés
                );
        */
        public async Task<IEnumerable<Asteroid>> SearchAsync(
            string? reference = null,
            int? orbitalClassId = null,
            bool? isPotentiallyHazardous = null,
            int? orbitId = null,
            decimal? absoluteMagnitude = null,
            decimal? minDiameter = null,
            decimal? maxDiameter = null,
            decimal? minInclination = null,
            decimal? maxInclination = null,
            decimal? semiMajorAxis = null,
            DateTime? firstObservationDate = null,
            DateTime? lastObservationDate = null)
        {
            var query = _asteroids.AsQueryable();

            if (!string.IsNullOrWhiteSpace(reference))
            {
                string refLower = reference.ToLower();
                query = query.Where(a => a.Reference != null && a.Reference.ToLower().Contains(refLower));
            }

            if (orbitalClassId.HasValue)
                query = query.Where(a => a.OrbitalClassId == orbitalClassId.Value);

            if (orbitId.HasValue)
                query = query.Where(a => a.OrbitId == orbitId.Value);

            if (isPotentiallyHazardous.HasValue)
                query = query.Where(a => a.IsPotentiallyHazardous == isPotentiallyHazardous.Value);

            if (minDiameter.HasValue)
                query = query.Where(a => a.DiameterMinKm >= minDiameter.Value);

            if (maxDiameter.HasValue)
                query = query.Where(a => a.DiameterMaxKm <= maxDiameter.Value);

            if (minInclination.HasValue)
                query = query.Where(a => a.Inclination >= minInclination.Value);

            if (maxInclination.HasValue)
                query = query.Where(a => a.Inclination <= maxInclination.Value);

            if (absoluteMagnitude.HasValue)
                query = query.Where(a => a.AbsoluteMagnitude == absoluteMagnitude.Value);

            if (semiMajorAxis.HasValue)
                query = query.Where(a => a.SemiMajorAxis == semiMajorAxis.Value);

            if (firstObservationDate.HasValue)
                query = query.Where(a => a.FirstObservationDate == firstObservationDate.Value);

            if (lastObservationDate.HasValue)
                query = query.Where(a => a.LastObservationDate == lastObservationDate.Value);

            return await query
                .Include(a => a.CelestialBodyNavigation)
                .Include(a => a.OrbitalClassNavigation)
                .ToListAsync();
        }
    }
}