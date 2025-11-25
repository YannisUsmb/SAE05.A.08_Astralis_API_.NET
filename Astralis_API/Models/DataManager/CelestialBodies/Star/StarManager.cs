using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class StarManager : DataManager<Star, int, string>, IStarRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Star> _stars;

        public StarManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _stars = _context.Set<Star>();
        }

        public async override Task<IEnumerable<Star>> GetByKeyAsync(string reference)
        {
            return await _stars.Where(s => s.CelestialBodyNavigation.Name.ToLower().Contains(reference.ToLower()))
                            .Include(s => s.CelestialBodyNavigation)
                            .Include(s => s.SpectralClassNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Star>> GetBySpectralClassIdAsync(int id)
        {
            return await _stars.Where(s => s.SpectralClassId == id)
                            .Include(s => s.CelestialBodyNavigation)
                            .Include(s => s.SpectralClassNavigation)
                            .ToListAsync();
        }
    }
}