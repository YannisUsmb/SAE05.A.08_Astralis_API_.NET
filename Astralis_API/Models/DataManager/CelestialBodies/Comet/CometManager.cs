using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CometManager : DataManager<Comet, int, string>, ISearchRepository<Comet, string>
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Comet> _comet;

        public CometManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _comet = _context.Set<Comet>();
        }

        public async Task<IEnumerable<Comet>> GetByKeyAsync(string reference)
        {
            return await _comet.Where(cb => cb.Reference.ToLower().Contains(reference.ToLower()))
                            .Include(cb => cb.CelestialBodyNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Comet>> GetByCategoryIdAsync(int id)
        {
            return await _comet.Where(cb => cb.Id == id)
                            .Include(cb => cb.CelestialBodyNavigation)
                            .ToListAsync();
        }
    }
}