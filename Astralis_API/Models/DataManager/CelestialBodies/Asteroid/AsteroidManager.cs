using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AsteroidManager : CrudManager<Asteroid, int, string>, ISearchRepository<Asteroid, string>
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Asteroid> _asteroid;

        public AsteroidManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _asteroid = _context.Set<Asteroid>();
        }

        public async Task<IEnumerable<Asteroid>> GetByKeyAsync(string reference)
        {
            return await _asteroid.Where(cb => cb.Reference.ToLower().Contains(reference.ToLower()))
                            .Include(cb => cb.CelestialBodyNavigation)
                            .Include(cb => cb.OrbitalClassNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Asteroid>> GetByCategoryIdAsync(int id)
        {
            return await _asteroid.Where(cb => cb.OrbitalClassId == id)
                            .Include(cb => cb.CelestialBodyNavigation)
                            .Include(cb => cb.OrbitalClassNavigation)
                            .ToListAsync();
        }
    }
}