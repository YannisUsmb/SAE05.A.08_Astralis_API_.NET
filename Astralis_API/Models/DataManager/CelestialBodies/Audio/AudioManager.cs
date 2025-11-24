using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AudioManager : DataManager<Audio, int, string>, ISearchRepository<Audio, string>
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Audio> _audio;

        public AudioManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _audio = _context.Set<Audio>();
        }

        public async Task<IEnumerable<Audio>> GetByKeyAsync(string title)
        {
            return await _audio.Where(cb => cb.Title.ToLower().Contains(title.ToLower()))
                            .Include(cb => cb.CelestialBodyTypeNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Audio>> GetByCategoryIdAsync(int id)
        {
            return await _audio.Where(cb => cb.CelestialBodyTypeId == id)
                            .Include(cb => cb.CelestialBodyTypeNavigation)
                            .ToListAsync();
        }
    }
}