using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class StarManager : DataManager<Star, int, string>, IStarRepository
    {
        public StarManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<Star?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(s => s.Id == id);
        }

        protected override IQueryable<Star> WithIncludes(IQueryable<Star> query)
        {
            return query.Include(s => s.CelestialBodyNavigation)
                        .Include(s => s.SpectralClassNavigation);
        }

        public async override Task<IEnumerable<Star>> GetByKeyAsync(string search)
        {
            return await WithIncludes(_entities.Where(s => 
                s.CelestialBodyNavigation.Name.ToLower().Contains(search) ||
                (s.Designation != null && s.Designation.ToLower().Contains(search)) || 
                (s.BayerDesignation != null && s.BayerDesignation.ToLower().Contains(search))
            )).ToListAsync();
        }

        public async Task<IEnumerable<Star>> SearchAsync(
            string? name = null,
            IEnumerable<int>? spectralClassIds = null,
            string? constellation = null,
            string? designation = null,
            string? bayerDesignation = null,
            decimal? minDistance = null,
            decimal? maxDistance = null,
            decimal? minLuminosity = null,
            decimal? maxLuminosity = null,
            decimal? minRadius = null,
            decimal? maxRadius = null,
            decimal? minTemperature = null,
            decimal? maxTemperature = null)
        {
            var query = _entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                string nameLower = name.ToLower();
                query = query.Where(s => s.CelestialBodyNavigation.Name.ToLower().Contains(nameLower));
            }

            if (spectralClassIds != null && spectralClassIds.Any())
                query = query.Where(s => s.SpectralClassId.HasValue && spectralClassIds.Contains(s.SpectralClassId.Value));

            if (!string.IsNullOrWhiteSpace(constellation))
                query = query.Where(s => s.Constellation != null && s.Constellation.ToLower().Contains(constellation.ToLower()));

            if (!string.IsNullOrWhiteSpace(designation))
                query = query.Where(s => s.Designation != null && s.Designation.ToLower().Contains(designation.ToLower()));

            if (!string.IsNullOrWhiteSpace(bayerDesignation))
                query = query.Where(s => s.BayerDesignation != null && s.BayerDesignation.ToLower().Contains(bayerDesignation.ToLower()));
                      
            if (minDistance.HasValue)
                query = query.Where(s => s.Distance >= minDistance.Value);

            if (maxDistance.HasValue)
                query = query.Where(s => s.Distance <= maxDistance.Value);

            if (minLuminosity.HasValue)
                query = query.Where(s => s.Luminosity >= minLuminosity.Value);

            if (maxLuminosity.HasValue)
                query = query.Where(s => s.Luminosity <= maxLuminosity.Value);

            if (minRadius.HasValue)
                query = query.Where(s => s.Radius >= minRadius.Value);

            if (maxRadius.HasValue)
                query = query.Where(s => s.Radius <= maxRadius.Value);

            if (minTemperature.HasValue)
                query = query.Where(s => s.Temperature >= minTemperature.Value);

            if (maxTemperature.HasValue)
                query = query.Where(s => s.Temperature <= maxTemperature.Value);

            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}