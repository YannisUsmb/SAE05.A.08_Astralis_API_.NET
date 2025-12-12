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

        // --- C'EST ICI QUE TOUT SE JOUE ---
        public async Task<IEnumerable<CelestialBody>> SearchAsync(
            string? searchText = null,
            IEnumerable<int>? celestialBodyTypeIds = null,
            bool? isDiscovery = null,
            int pageNumber = 1,  // <--- Ajouté
            int pageSize = 30)  // <--- Ajouté
        {
            var query = _entities.AsQueryable();

            // 1. Filtres
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

            // 2. Pagination
            // On applique WithIncludes pour avoir les données liées
            // IMPORTANT : On ajoute OrderBy, sinon le Skip peut planter ou être aléatoire
            return await WithIncludes(query)
                .OrderBy(cb => cb.Name)            // Tri alphabétique par défaut
                .Skip((pageNumber - 1) * pageSize) // On saute les pages précédentes
                .Take(pageSize)                    // On prend le paquet demandé
                .ToListAsync();
        }
    }
}