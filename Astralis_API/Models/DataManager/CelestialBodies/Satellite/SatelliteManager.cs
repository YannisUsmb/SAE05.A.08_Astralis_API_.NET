using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class SatelliteManager : DataManager<Satellite, int, string>, ISatelliteRepository
    {
        public SatelliteManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<Satellite?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(s => s.Id == id);
        }

        protected override IQueryable<Satellite> WithIncludes(IQueryable<Satellite> query)
        {
            return query.Include(s => s.CelestialBodyNavigation)
                        .Include(s => s.PlanetNavigation)
                            .ThenInclude(p => p.CelestialBodyNavigation);
        }

        public async override Task<IEnumerable<Satellite>> GetByKeyAsync(string reference)
        {
            return await WithIncludes(_entities.Where(s => s.CelestialBodyNavigation.Name.ToLower()
                            .Contains(reference.ToLower()))).ToListAsync();
        }

        public async Task<IEnumerable<Satellite>> SearchAsync(
            string? name = null,
            IEnumerable<int>? planetIds = null,
            decimal? minGravity = null,
            decimal? maxGravity = null,
            decimal? minRadius = null,
            decimal? maxRadius = null,
            decimal? minDensity = null,
            decimal? maxDensity = null)
        {
            var query = _entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))            
                query = query.Where(s => s.CelestialBodyNavigation.Name.ToLower().Contains(name.ToLower()));          
            
            if (planetIds != null && planetIds.Any())            
                query = query.Where(a => planetIds.Contains(a.PlanetId));
            
            if (minGravity.HasValue)
                query = query.Where(s => s.Gravity >= minGravity.Value);

            if (maxGravity.HasValue)
                query = query.Where(s => s.Gravity <= maxGravity.Value);

            if (minRadius.HasValue)
                query = query.Where(s => s.Radius >= minRadius.Value);

            if (maxRadius.HasValue)
                query = query.Where(s => s.Radius <= maxRadius.Value);

            if (minDensity.HasValue)
                query = query.Where(s => s.Density >= minDensity.Value);

            if (maxDensity.HasValue)
                query = query.Where(s => s.Density <= maxDensity.Value);

            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}