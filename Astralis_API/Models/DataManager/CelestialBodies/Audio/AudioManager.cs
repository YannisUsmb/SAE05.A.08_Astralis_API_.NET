using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AudioManager : ReadableManager<Audio, int>, IAudioRepository
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
        public async Task<IEnumerable<Audio>> GetByKeyAsync(string title)
        {
            return await WithIncludes(_entities.Where(a => a.Title.ToLower().Contains(title.ToLower())))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Audio?>> SearchAsync(
                    string? searchTerm = null,
                    IEnumerable<int>? celestialBodyTypeIds = null)
        {
            var query = _entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower();
                query = query.Where(a =>
                    a.Title.ToLower().Contains(searchTermLower) || a.Description.ToLower().Contains(searchTermLower)
                );
            }

            if (celestialBodyTypeIds != null && celestialBodyTypeIds.Any())
            {
                query = query.Where(a => celestialBodyTypeIds.Contains(a.CelestialBodyTypeId));
            }

            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}