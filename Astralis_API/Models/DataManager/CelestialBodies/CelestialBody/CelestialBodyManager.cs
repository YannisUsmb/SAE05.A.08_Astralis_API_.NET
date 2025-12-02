using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CelestialBodyManager : DataManager<CelestialBody, int, string>, ICelestialBodyRepository
    {
        
        public CelestialBodyManager(AstralisDbContext context):base(context)
        {
        }

        public override async Task<CelestialBody?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities).FirstOrDefaultAsync(cb => cb.Id == id);
        }

        protected override IQueryable<CelestialBody> WithIncludes(IQueryable<CelestialBody> query)
        {
            return query
                .Include(cb => cb.PlanetNavigation)
                .Include(cb => cb.GalaxyQuasarNavigation)
                .Include(cb => cb.StarNavigation)
                .Include(cb => cb.SatelliteNavigation)
                .Include(cb => cb.DiscoveryNavigation)
                .Include(cb => cb.CelestialBodyTypeNavigation)
                .Include(cb => cb.AsteroidNavigation)
                .Include(cb => cb.CometNavigation);
        }


        public async override Task<IEnumerable<CelestialBody>> GetByKeyAsync(string name)
        {
            return await WithIncludes(_entities.Where(cb =>
                cb.Name.ToLower().Contains(name.ToLower())
                || (cb.Alias != null && cb.Alias.ToLower().Contains(name.ToLower()))
            )).ToListAsync();
        }

        public async Task<IEnumerable<CelestialBody>> SearchAsync(
            string? searchText = null,
            IEnumerable<int>? celestialBodyTypeIds = null,
            bool? isDiscovery = null)
        {
            var query = _entities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string lower = searchText.ToLower();
                query = query.Where(cb => cb.Name.ToLower().Contains(lower)
                                       || (cb.Alias != null && cb.Alias.ToLower().Contains(lower)));
            }

            if (celestialBodyTypeIds != null && celestialBodyTypeIds.Any())
            {
                query = query.Where(cb => celestialBodyTypeIds.Contains(cb.CelestialBodyTypeId));
            }

            if (isDiscovery.HasValue)
            {
                if (isDiscovery.Value == true)
                {
                    query = query.Where(cb => cb.DiscoveryNavigation != null);
                }
                else
                {
                    query = query.Where(cb => cb.DiscoveryNavigation == null);
                }
            }
            return await WithIncludes(query).ToListAsync();
        }
    }
}