using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AudioManager : DataManager<Audio, int, string>, IAudioRepository
    {
        public AudioManager(AstralisDbContext context) : base(context)
        {
        }
        public override async Task<Audio?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(c => c.Id == id);
        }

        protected override IQueryable<Audio> WithIncludes(IQueryable<Audio> query)
        {
            return query
                .Include(a => a.CelestialBodyTypeNavigation);
        }
        public async override Task<IEnumerable<Audio>> GetByKeyAsync(string title)
        {
            return await WithIncludes(_entities.Where(a => a.Title.ToLower().Contains(title.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Audio>> GetByCategoryIdAsync(int id)
        {
            return await WithIncludes(_entities.Where(a => a.CelestialBodyTypeId == id))
                            .ToListAsync();
        }
    }
}