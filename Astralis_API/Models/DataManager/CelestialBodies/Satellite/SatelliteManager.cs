using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class SatelliteManager : DataManager<Satellite, int, string>, ISatelliteRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Satellite> _satellites;

        public SatelliteManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _satellites = _context.Set<Satellite>();
        }

        public async override Task<IEnumerable<Satellite>> GetByKeyAsync(string reference)
        {
            return await _satellites.Where(s => s.CelestialBodyNavigation.Name.ToLower().Contains(reference.ToLower()))
                            .Include(s => s.CelestialBodyNavigation)
                            .Include(s => s.PlanetNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Satellite>> GetByPlanetIdAsync(int id)
        {
            return await _satellites.Where(s => s.PlanetId == id)
                            .Include(s => s.CelestialBodyNavigation)
                            .Include(s => s.PlanetNavigation)
                            .ToListAsync();
        }
    }
}