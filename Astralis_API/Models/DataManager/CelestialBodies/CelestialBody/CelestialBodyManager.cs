using System.Linq.Expressions;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CelestialBodyManager : DataManager<CelestialBody, int, string>, ICelestialBodyRepository
    {
        private readonly Dictionary<int, Func<Task<List<CelestialBodySubtypeDto>>>> _subtypeStrategies;
        private readonly Dictionary<int, Func<IQueryable<CelestialBody>, CelestialBodyFilterDto, IQueryable<CelestialBody>>> _specificFilterStrategies;
        private readonly Dictionary<string, Func<IQueryable<CelestialBody>, bool, IQueryable<CelestialBody>>> _sortingStrategies;
        
        public CelestialBodyManager(AstralisDbContext context):base(context)
        {
            _subtypeStrategies = new Dictionary<int, Func<Task<List<CelestialBodySubtypeDto>>>>
            {
                { 1, () => GetSubtypesFromDbSet(_context.SpectralClasses, x => x.Id, x => x.Label, x => x.Description) },
                { 2, () => GetSubtypesFromDbSet(_context.PlanetTypes, x => x.Id, x => x.Label, x => x.Description) },
                { 3, () => GetSubtypesFromDbSet(_context.OrbitalClasses, x => x.Id, x => x.Label, x => x.Description) },
                { 5, () => GetSubtypesFromDbSet(_context.GalaxyQuasarClasses, x => x.Id, x => x.Label, x => x.Description) }
            };
            
            _specificFilterStrategies = new Dictionary<int, Func<IQueryable<CelestialBody>, CelestialBodyFilterDto, IQueryable<CelestialBody>>>
            {
                { 1, (q, dto) => ApplyStarFilters(q, dto.StarFilter) },   
                { 2, (q, dto) => ApplyPlanetFilters(q, dto.PlanetFilter) },
                { 3, (q, dto) => ApplyAsteroidFilters(q, dto.AsteroidFilter) },
                {4 , (q, dto) => ApplySatelliteFilters(q, dto.SatelliteFilter) },
                { 5, (q, dto) => ApplyGalaxyFilters(q, dto.GalaxyFilter) },
                { 6, (q, dto) => ApplyCometFilters(q, dto.CometFilter) }
            };
            
            _sortingStrategies = new Dictionary<string, Func<IQueryable<CelestialBody>, bool, IQueryable<CelestialBody>>>(StringComparer.OrdinalIgnoreCase)
            {
                { "name", (q, asc) => ApplySort(q, asc, cb => cb.Name) },
        
                { "distance", (q, asc) => ApplySort(q, asc, cb => 
                    cb.PlanetNavigation != null ? (double?)cb.PlanetNavigation.Distance : 
                    cb.StarNavigation != null ? (double?)cb.StarNavigation.Distance :
                    null) 
                },
        
                { "radius", (q, asc) => ApplySort(q, asc, cb => 
                    cb.PlanetNavigation != null ? (double?)cb.PlanetNavigation.Radius :
                    cb.StarNavigation != null ? (double?)cb.StarNavigation.Radius :
                    cb.SatelliteNavigation != null ? (double?)cb.SatelliteNavigation.Radius :
                    cb.AsteroidNavigation != null ? (double?)(cb.AsteroidNavigation.DiameterMaxKm / (decimal?)2.0) :
                    null)
                },

                { "mass", (q, asc) => ApplySort(q, asc, cb => 
                    cb.PlanetNavigation != null ? (double?)cb.PlanetNavigation.Mass : null) 
                },

                { "temperature", (q, asc) => ApplySort(q, asc, cb => 
                    cb.PlanetNavigation != null 
                        // Correction ici : On utilise Convert.ToDouble car c'est un string en base
                        ? Convert.ToDouble(cb.PlanetNavigation.Temperature) 
                        : cb.StarNavigation != null 
                            ? (double?)cb.StarNavigation.Temperature 
                            : null)
                }
            };
        }
        
        private async Task<List<CelestialBodySubtypeDto>> GetSubtypesFromDbSet<T>(
            DbSet<T> dbSet, 
            Func<T, int> idSelector, 
            Func<T, string> labelSelector,
            Func<T, string?> descriptionSelector) where T : class
        {
            var list = await dbSet.ToListAsync();
            return list.Select(item => new CelestialBodySubtypeDto
            {
                Id = idSelector(item),
                Label = labelSelector(item),
                Description = descriptionSelector(item)
            }).ToList();
        }
        
        public override async Task<CelestialBody?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities).FirstOrDefaultAsync(cb => cb.Id == id);
        }

        protected override IQueryable<CelestialBody> WithIncludes(IQueryable<CelestialBody> query)
        {
            return query
                .Include(cb => cb.CelestialBodyTypeNavigation)
                .Include(cb => cb.StarNavigation)
                .ThenInclude(s => s.SpectralClassNavigation) 
                .Include(cb => cb.PlanetNavigation)
                .ThenInclude(p => p.PlanetTypeNavigation)
                .Include(cb => cb.PlanetNavigation)
                .ThenInclude(p => p.DetectionMethodNavigation)
                .Include(cb => cb.AsteroidNavigation)
                .ThenInclude(a => a.OrbitalClassNavigation)
                .Include(cb => cb.SatelliteNavigation)
                .ThenInclude(s => s.PlanetNavigation)
                .ThenInclude(p => p.CelestialBodyNavigation)
                .Include(cb => cb.GalaxyQuasarNavigation)
                .ThenInclude(g => g.GalaxyQuasarClassNavigation)
                .Include(cb => cb.CometNavigation)
                .Include(cb => cb.DiscoveryNavigation);
        }

        public async override Task<IEnumerable<CelestialBody>> GetByKeyAsync(string name)
        {
            return await WithIncludes(_entities.Where(cb =>
                cb.Name.ToLower().Contains(name.ToLower())
                || (cb.Alias != null && cb.Alias.ToLower().Contains(name.ToLower()))
            )).ToListAsync();
        }
        
        public async Task<List<CelestialBodySubtypeDto>> GetSubtypesByMainTypeAsync(int mainTypeId)
        {
            if (_subtypeStrategies.TryGetValue(mainTypeId, out var strategy))
            {
                return await strategy();
            }
    
            return new List<CelestialBodySubtypeDto>();
        }
        
        public async Task<IEnumerable<DetectionMethod>> GetDetectionMethodsAsync()
        {
            return await _context.DetectionMethods.ToListAsync();
        }
        
        public async Task<IEnumerable<CelestialBody>> SearchAsync(
            CelestialBodyFilterDto filter,
            int pageNumber = 1,
            int pageSize = 30)
        {
            var query = _entities.AsQueryable();
            
            query = query.Where(cb => cb.DiscoveryNavigation == null || cb.DiscoveryNavigation.DiscoveryStatusId == 3);
            
            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                string lower = filter.SearchText.ToLower();
                query = query.Where(cb => cb.Name.ToLower().Contains(lower) 
                                          || (cb.Alias != null && cb.Alias.ToLower().Contains(lower)));
            }

            if (filter.CelestialBodyTypeIds != null && filter.CelestialBodyTypeIds.Any())
            {
                query = query.Where(cb => filter.CelestialBodyTypeIds.Contains(cb.CelestialBodyTypeId));
                
                if (filter.CelestialBodyTypeIds.Count == 1)
                {
                    int typeId = filter.CelestialBodyTypeIds.First();
                    if (_specificFilterStrategies.TryGetValue(typeId, out var filterFunc))
                    {
                        query = filterFunc(query, filter);
                    }
                }
            }

            if (filter.IsDiscovery.HasValue)
            {
                query = filter.IsDiscovery.Value 
                    ? query.Where(cb => cb.DiscoveryNavigation != null) 
                    : query.Where(cb => cb.DiscoveryNavigation == null);
            }

            if (filter.SubtypeId.HasValue && filter.SubtypeId.Value > 0)
            {
                query = query.Where(cb => 
                    (cb.PlanetNavigation != null && cb.PlanetNavigation.PlanetTypeId == filter.SubtypeId) ||
                    (cb.GalaxyQuasarNavigation != null && cb.GalaxyQuasarNavigation.GalaxyQuasarClassId == filter.SubtypeId) ||
                    (cb.AsteroidNavigation != null && cb.AsteroidNavigation.OrbitalClassId == filter.SubtypeId) ||
                    (cb.StarNavigation != null && cb.StarNavigation.SpectralClassId == filter.SubtypeId)
                );
            }
            
            var sortKey = string.IsNullOrWhiteSpace(filter.SortBy) ? "name" : filter.SortBy;

            if (!_sortingStrategies.TryGetValue(sortKey, out var sortFunc))
            {
                sortFunc = _sortingStrategies["name"];
            }

            query = sortFunc(query, filter.SortAscending);
            
            return await WithIncludes(query)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)                    
                .ToListAsync();
        }
        
        private IQueryable<CelestialBody> ApplyPlanetFilters(IQueryable<CelestialBody> query, PlanetFilterDto? f)
        {
            if (f == null) return query;

            query = query.Where(cb => cb.PlanetNavigation != null); 
            
            if (f.MinMass.HasValue) query = query.Where(cb => cb.PlanetNavigation.Mass >= f.MinMass);
            if (f.MaxMass.HasValue) query = query.Where(cb => cb.PlanetNavigation.Mass <= f.MaxMass);
            
            if (f.MinDistance.HasValue) query = query.Where(cb => cb.PlanetNavigation.Distance >= f.MinDistance);
            if (f.MaxDistance.HasValue) query = query.Where(cb => cb.PlanetNavigation.Distance <= f.MaxDistance);
            
            if (f.MinRadius.HasValue) query = query.Where(cb => cb.PlanetNavigation.Radius >= f.MinRadius);
            if (f.MaxRadius.HasValue) query = query.Where(cb => cb.PlanetNavigation.Radius <= f.MaxRadius);

            return query;
        }

        private IQueryable<CelestialBody> ApplyStarFilters(IQueryable<CelestialBody> query, StarFilterDto? f)
        {
            if (f == null) return query;
            query = query.Where(cb => cb.StarNavigation != null);

            if (f.MinTemperature.HasValue) query = query.Where(cb => cb.StarNavigation.Temperature >= f.MinTemperature);
            if (f.MaxTemperature.HasValue) query = query.Where(cb => cb.StarNavigation.Temperature <= f.MaxTemperature);
            
            if (f.MinDistance.HasValue) query = query.Where(cb => cb.StarNavigation.Distance >= f.MinDistance);
            if (f.MaxDistance.HasValue) query = query.Where(cb => cb.StarNavigation.Distance <= f.MaxDistance);

            return query;
        }

        private IQueryable<CelestialBody> ApplyAsteroidFilters(IQueryable<CelestialBody> query, AsteroidFilterDto? f)
        {
            if (f == null) return query;
            query = query.Where(cb => cb.AsteroidNavigation != null);

            if (f.MinDiameter.HasValue) query = query.Where(cb => cb.AsteroidNavigation.DiameterMaxKm >= f.MinDiameter);
            if (f.MaxDiameter.HasValue) query = query.Where(cb => cb.AsteroidNavigation.DiameterMaxKm <= f.MaxDiameter);
            
            if (f.IsPotentiallyHazardous.HasValue) 
                query = query.Where(cb => cb.AsteroidNavigation.IsPotentiallyHazardous == f.IsPotentiallyHazardous);

            return query;
        }

        private IQueryable<CelestialBody> ApplySatelliteFilters(IQueryable<CelestialBody> query, SatelliteFilterDto? f)
        {
            if (f == null) return query;
            query = query.Where(cb => cb.SatelliteNavigation != null);
            
            return query;
        }

        private IQueryable<CelestialBody> ApplyGalaxyFilters(IQueryable<CelestialBody> query, GalaxyQuasarFilterDto? f)
        {
            if (f == null) return query;
            query = query.Where(cb => cb.GalaxyQuasarNavigation != null);

            if (f.MinRedshift.HasValue) query = query.Where(cb => cb.GalaxyQuasarNavigation.Redshift >= f.MinRedshift);
            
            return query;
        }

        private IQueryable<CelestialBody> ApplyCometFilters(IQueryable<CelestialBody> query, CometFilterDto? f)
        {
            if (f == null) return query;
            query = query.Where(cb => cb.CometNavigation != null);

            if (f.MinEccentricity.HasValue) query = query.Where(cb => cb.CometNavigation.OrbitalEccentricity >= f.MinEccentricity);
            return query;
        }

        private static IQueryable<CelestialBody> ApplySort<TKey>(
            IQueryable<CelestialBody> query, 
            bool asc, 
            Expression<Func<CelestialBody, TKey>> keySelector)
        {
            return asc ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector);
        }
    }
}