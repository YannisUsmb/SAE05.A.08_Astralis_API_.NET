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

        public async override Task<IEnumerable<Planet>> GetByKeyAsync(string reference)
        {
            return await _planets.Where(p => p.CelestialBodyNavigation.Name.ToLower().Contains(reference.ToLower()))
                            .Include(p => p.CelestialBodyNavigation)
                            .Include(p => p.DetectionMethodNavigation)
                            .Include(p => p.PlanetTypeNavigation)
                            .Include(p => p.Satellites)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Planet>> GetByPlanetTypeIdAsync(int id)
        {
            return await _planets.Where(p => p.PlanetTypeId == id)
                            .Include(p => p.CelestialBodyNavigation)
                            .Include(p => p.DetectionMethodNavigation)
                            .Include(p => p.PlanetTypeNavigation)
                            .Include(p => p.Satellites)
                            .ToListAsync();
        }
    }
}