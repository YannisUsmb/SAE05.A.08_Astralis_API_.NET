using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CometManager : DataManager<Comet, int, string>, ICometRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Comet> _comets;

        public CometManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _comets = _context.Set<Comet>();
        }

        public async override Task<IEnumerable<Comet>> GetByKeyAsync(string reference)
        {
            return await _comets.Where(c => c.Reference.ToLower().Contains(reference.ToLower()))
                            .Include(c => c.CelestialBodyNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Comet>> GetByCategoryIdAsync(string reference)
        {
            return await _comets.Where(c => c.Reference == reference)
                            .Include(c => c.CelestialBodyNavigation)
                            .ToListAsync();
        }
    }
}