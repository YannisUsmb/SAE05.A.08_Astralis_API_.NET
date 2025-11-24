using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CelestialBodyManager : DataManager<CelestialBody, int, string>, ICelestialBodyRepository, ISearchRepository<CelestialBody, string>
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<CelestialBody> _celestialBodies;

        public CelestialBodyManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _celestialBodies = _context.Set<CelestialBody>();
        }

        public async Task<IEnumerable<CelestialBody>> GetByKeyAsync(string name)
        {
            return await _celestialBodies.Where(cb=> cb.Name.ToLower().Contains(name.ToLower()))
                            .Include(cb=> cb.PlanetNavigation)
                            .Include(cb=> cb.GalaxyQuasarNavigation)
                            .Include(cb=> cb.StarNavigation)
                            .Include(cb=> cb.SatelliteNavigation)
                            .Include(cb=> cb.DiscoveryNavigation)
                            .Include(cb=> cb.CelestialBodyTypeNavigation)
                            .Include(cb=> cb.AsteroidNavigation)
                            .Include(cb=> cb.CometNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<CelestialBody>> GetByCategoryIdAsync(int id)
        {
            return await _celestialBodies.Where(cb=> cb.CelestialBodyTypeId == id)
                            .Include(cb=> cb.PlanetNavigation)
                            .Include(cb=> cb.GalaxyQuasarNavigation)
                            .Include(cb=> cb.StarNavigation)
                            .Include(cb=> cb.SatelliteNavigation)
                            .Include(cb=> cb.DiscoveryNavigation)
                            .Include(cb=> cb.CelestialBodyTypeNavigation)
                            .Include(cb=> cb.AsteroidNavigation)
                            .Include(cb=> cb.CometNavigation)
                            .ToListAsync();
        }
    }
}