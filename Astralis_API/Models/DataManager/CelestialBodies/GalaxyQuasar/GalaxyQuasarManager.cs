using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class GalaxyQuasarManager : DataManager<GalaxyQuasar, int, string>, IGalaxyQuasarRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<GalaxyQuasar> _galaxyQuasars;

        public GalaxyQuasarManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _galaxyQuasars = _context.Set<GalaxyQuasar>();
        }

        public async override Task<IEnumerable<GalaxyQuasar>> GetByKeyAsync(string reference)
        {
            return await _galaxyQuasars.Where(gq => gq.Reference.ToLower().Contains(reference.ToLower()))
                            .Include(gq => gq.CelestialBodyNavigation)
                            .Include(gq => gq.GalaxyQuasarClassNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<GalaxyQuasar>> GetByGalaxyQuasarClassIdAsync(int id)
        {
            return await _galaxyQuasars.Where(gq => gq.GalaxyQuasarClassId == id)
                            .Include(gq => gq.CelestialBodyNavigation)
                            .Include(gq => gq.GalaxyQuasarClassNavigation)
                            .ToListAsync();
        }
    }
}