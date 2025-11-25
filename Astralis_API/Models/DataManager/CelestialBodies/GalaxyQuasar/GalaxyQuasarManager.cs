using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class GalaxyQuasarManager : DataManager<GalaxyQuasar, int, string>, IGalaxyQuasarRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<GalaxyQuasar> _GalaxyQuasar;

        public GalaxyQuasarManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _GalaxyQuasar = _context.Set<GalaxyQuasar>();
        }

        public async override Task<IEnumerable<GalaxyQuasar>> GetByKeyAsync(string reference)
        {
            return await _GalaxyQuasar.Where(d => d.Reference.ToLower().Contains(reference.ToLower()))
                            .Include(d => d.CelestialBodyNavigation)
                            .Include(d => d.GalaxyQuasarClassNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<GalaxyQuasar>> GetByCategoryIdAsync(int id)
        {
            return await _GalaxyQuasar.Where(d => d.GalaxyQuasarClassId == id)
                            .Include(d => d.CelestialBodyNavigation)
                            .Include(d => d.GalaxyQuasarClassNavigation)
                            .ToListAsync();
        }
    }
}