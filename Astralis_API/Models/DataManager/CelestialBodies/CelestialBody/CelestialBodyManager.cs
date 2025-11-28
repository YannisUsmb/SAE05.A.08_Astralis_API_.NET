using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CelestialBodyManager : DataManager<CelestialBody, int, string>, ICelestialBodyRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<CelestialBody> _celestialBodies;

        public CelestialBodyManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _celestialBodies = _context.Set<CelestialBody>();
        }
        protected override IQueryable<CelestialBody> WithIncludes(IQueryable<CelestialBody> query)
        {
            return query
                            .Include(cb => cb.PlanetNavigation)
                            .Include(cb => cb.GalaxyQuasarNavigation)
                            .Include(cb => cb.StarNavigation)
                            .Include(cb => cb.SatelliteNavigation)
                            .Include(cb => cb.DiscoveryNavigation)
                            .Include(cb => cb.CelestialBodyTypeNavigation)
                            .Include(cb => cb.AsteroidNavigation)
                            .Include(cb => cb.CometNavigation);
        }

        public async override Task<IEnumerable<CelestialBody>> GetByKeyAsync(string name)
        {
            return await WithIncludes(_celestialBodies.Where(cb => cb.Name.ToLower().Contains(name)
                          || (cb.Alias != null && cb.Alias.ToLower().Contains(name))))
                            .ToListAsync();
        }

        public async Task<IEnumerable<CelestialBody>> GetByCelestialBodyTypeIdAsync(int id)
        {
            return await WithIncludes(_celestialBodies.Where(cb => cb.CelestialBodyTypeId == id))
                            .ToListAsync();
        }

        public async Task<IEnumerable<CelestialBody>> GetDiscoveriesAsync()
        {
            return await WithIncludes(_celestialBodies
                .Where(cb => cb.DiscoveryNavigation != null))
                .ToListAsync();
        }
    }
}