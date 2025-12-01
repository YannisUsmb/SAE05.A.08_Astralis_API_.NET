using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class PlanetManager : DataManager<Planet, int, string>, IPlanetRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Planet> _planets;

        public PlanetManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _planets = _context.Set<Planet>();
        }

        protected override IQueryable<Planet> WithIncludes(IQueryable<Planet> query)
        {
            return query.Include(p => p.CelestialBodyNavigation)
                            .Include(p => p.DetectionMethodNavigation)
                            .Include(p => p.PlanetTypeNavigation)
                            .Include(p => p.Satellites);
        }

        public async override Task<IEnumerable<Planet>> GetByKeyAsync(string reference)
        {
            return await WithIncludes(_planets.Where(p => p.CelestialBodyNavigation.Name.ToLower().Contains(reference.ToLower())))
                            .ToListAsync();
        }
        public async Task<IEnumerable<Planet>> SearchAsync(
            string? name = null,
            IEnumerable<int>? planetTypeIds = null,
            int? detectionMethodId = null,
            int? minDiscoveryYear = null,
            int? maxDiscoveryYear = null,
            decimal? minOrbitalPeriod = null,
            decimal? maxOrbitalPeriod = null,
            decimal? minEccentricity = null,
            decimal? maxEccentricity = null,
            decimal? minStellarMagnitude = null,
            decimal? maxStellarMagnitude = null,
            string? distance = null,
            string? radius = null,
            string? temperature = null,
            string? hostStarTemperature = null,
            string? hostStarMass = null)
        {
            var query = _planets.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                string nameLower = name.ToLower();
                query = query.Where(p => p.CelestialBodyNavigation.Name.ToLower().Contains(nameLower));
            }

            if (planetTypeIds != null && planetTypeIds.Any())
            {
                query = query.Where(a => planetTypeIds.Contains(a.PlanetTypeId));
            }

            if (detectionMethodId.HasValue)
                query = query.Where(p => p.DetectionMethodId == detectionMethodId.Value);

            if (minDiscoveryYear.HasValue)
                query = query.Where(p => p.DiscoveryYear >= minDiscoveryYear.Value);

            if (maxDiscoveryYear.HasValue)
                query = query.Where(p => p.DiscoveryYear <= maxDiscoveryYear.Value);

            if (minOrbitalPeriod.HasValue)
                query = query.Where(p => p.OrbitalPeriod >= minOrbitalPeriod.Value);

            if (maxOrbitalPeriod.HasValue)
                query = query.Where(p => p.OrbitalPeriod <= maxOrbitalPeriod.Value);

            if (minEccentricity.HasValue)
                query = query.Where(p => p.Eccentricity >= minEccentricity.Value);

            if (maxEccentricity.HasValue)
                query = query.Where(p => p.Eccentricity <= maxEccentricity.Value);

            if (minStellarMagnitude.HasValue)
                query = query.Where(p => p.StellarMagnitude >= minStellarMagnitude.Value);

            if (maxStellarMagnitude.HasValue)
                query = query.Where(p => p.StellarMagnitude <= maxStellarMagnitude.Value);

            if (!string.IsNullOrWhiteSpace(distance))
                query = query.Where(p => p.Distance != null && p.Distance.ToLower().Contains(distance.ToLower()));

            if (!string.IsNullOrWhiteSpace(radius))
                query = query.Where(p => p.Radius != null && p.Radius.ToLower().Contains(radius.ToLower()));

            if (!string.IsNullOrWhiteSpace(temperature))
                query = query.Where(p => p.Temperature != null && p.Temperature.ToLower().Contains(temperature.ToLower()));

            if (!string.IsNullOrWhiteSpace(hostStarMass))
                query = query.Where(p => p.HostStarMass != null && p.HostStarMass.ToLower().Contains(hostStarMass.ToLower()));

            if (!string.IsNullOrWhiteSpace(hostStarTemperature))
                query = query.Where(p => p.HostStarTemperature != null && p.HostStarTemperature.ToLower().Contains(hostStarTemperature.ToLower()));

            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}