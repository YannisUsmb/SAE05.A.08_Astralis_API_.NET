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
    }
}