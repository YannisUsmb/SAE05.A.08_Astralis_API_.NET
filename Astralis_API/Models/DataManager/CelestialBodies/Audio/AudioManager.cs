using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AudioManager : DataManager<Audio, int, string>, IAudioRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Audio> _audios;

        public AudioManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _audios = _context.Set<Audio>();
        }
        protected override IQueryable<Audio> WithIncludes(IQueryable<Audio> query)
        {
            return query
                .Include(a => a.CelestialBodyTypeNavigation)
                ;
        }
        public async override Task<IEnumerable<Audio>> GetByKeyAsync(string title)
        {
            return await WithIncludes(_audios.Where(a => a.Title.ToLower().Contains(title.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Audio>> GetByCategoryIdAsync(int id)
        {
            return await WithIncludes(_audios.Where(a => a.CelestialBodyTypeId == id))
                            .ToListAsync();
        }
    }
}