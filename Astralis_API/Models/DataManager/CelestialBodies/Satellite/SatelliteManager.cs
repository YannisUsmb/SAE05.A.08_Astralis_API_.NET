using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class SatelliteManager : DataManager<Satellite, int, string>, ISatelliteRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Satellite> _satellites;

        public SatelliteManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _satellites = _context.Set<Satellite>();
        }

        public async override Task<IEnumerable<Satellite>> GetByKeyAsync(string reference)
        {
            return await _satellites.Where(s => s.CelestialBodyNavigation.Name.ToLower().Contains(reference.ToLower()))
                            .Include(s => s.CelestialBodyNavigation)
                            .Include(s => s.PlanetNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Satellite>> SearchAsync(
            string? name = null,
            int? planetId = null,
            int? celestialBodyId = null,
            decimal? minGravity = null,
            decimal? maxGravity = null,
            decimal? minRadius = null,
            decimal? maxRadius = null,
            decimal? minDensity = null,
            decimal? maxDensity = null)
        {
            var query = _satellites.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                string nameLower = name.ToLower();
                query = query.Where(s => s.CelestialBodyNavigation.Name.ToLower().Contains(nameLower));
            }
            if (planetId.HasValue)
                query = query.Where(s => s.PlanetId == planetId.Value);
            if (celestialBodyId.HasValue)
                query = query.Where(s => s.CelestialBodyId == celestialBodyId.Value);
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
            return await query
                .Include(s => s.CelestialBodyNavigation)
                .Include(s => s.PlanetNavigation)
                .ThenInclude(p => p.CelestialBodyNavigation)
                .ToListAsync();
        }
    }
}