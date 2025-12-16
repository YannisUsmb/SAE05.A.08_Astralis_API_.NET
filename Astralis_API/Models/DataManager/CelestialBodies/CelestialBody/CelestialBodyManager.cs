using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CelestialBodyManager : DataManager<CelestialBody, int, string>, ICelestialBodyRepository
    {
        private readonly Dictionary<int, Func<Task<IDictionary<int, string>>>> _subtypeStrategies;
        
        public CelestialBodyManager(AstralisDbContext context):base(context)
        {
            _subtypeStrategies = new Dictionary<int, Func<Task<IDictionary<int, string>>>>
            {
                { 1, () => GetSubtypesFromDbSet(_context.SpectralClasses, x => x.Id, x => x.Label) },
                { 2, () => GetSubtypesFromDbSet(_context.PlanetTypes, x => x.Id, x => x.Label) },
                { 3, () => GetSubtypesFromDbSet(_context.OrbitalClasses, x => x.Id, x => x.Label) },
                { 5, () => GetSubtypesFromDbSet(_context.GalaxyQuasarClasses, x => x.Id, x => x.Label) }
            };
        }
        
        private async Task<IDictionary<int, string>> GetSubtypesFromDbSet<T>(
            DbSet<T> dbSet, 
            Func<T, int> keySelector, 
            Func<T, string> valueSelector) where T : class
        {
             var list = await dbSet.ToListAsync();
            return list.ToDictionary(keySelector, valueSelector);
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
        
        public async Task<IDictionary<int, string>> GetSubtypesByMainTypeAsync(int mainTypeId)
        {
            if (_subtypeStrategies.TryGetValue(mainTypeId, out var strategy))
            {
                return await strategy();
            }
            
            return new Dictionary<int, string>();
        }
        
        public async Task<IEnumerable<CelestialBody>> SearchAsync(
            string? searchText = null,
            IEnumerable<int>? celestialBodyTypeIds = null,
            bool? isDiscovery = null,
            int? subtypeId = null,
            int pageNumber = 1,
            int pageSize = 30)
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

            if (subtypeId.HasValue && subtypeId.Value > 0)
            {
                query = query.Where(cb => 
                    (cb.PlanetNavigation != null && cb.PlanetNavigation.PlanetTypeId == subtypeId) ||
                    (cb.GalaxyQuasarNavigation != null && cb.GalaxyQuasarNavigation.GalaxyQuasarClassId == subtypeId) ||
                    (cb.AsteroidNavigation != null && cb.AsteroidNavigation.OrbitalClassId == subtypeId) ||
                    (cb.StarNavigation != null && cb.StarNavigation.SpectralClassId == subtypeId)
                );
            }
            
            return await WithIncludes(query)
                .OrderBy(cb => cb.Name)     
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)                    
                .ToListAsync();
        }
    }
}