using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class StarManager : DataManager<Star, int, string>, IStarRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Star> _stars;

        public StarManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _stars = _context.Set<Star>();
        }

        public async override Task<IEnumerable<Star>> GetByKeyAsync(string reference)
        {
            return await _stars.Where(s => s.CelestialBodyNavigation.Name.ToLower().Contains(reference.ToLower()))
                            .Include(s => s.CelestialBodyNavigation)
                            .Include(s => s.SpectralClassNavigation)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Star>> SearchAsync(
            string? name = null,
            int? spectralClassId = null,
            int? celestialBodyId = null,
            string? constellation = null,
            string? designation = null,
            string? bayerDesignation = null,
            DateOnly? minApprovalDate = null,
            DateOnly? maxApprovalDate = null,
            decimal? minDistance = null,
            decimal? maxDistance = null,
            decimal? minLuminosity = null,
            decimal? maxLuminosity = null,
            decimal? minRadius = null,
            decimal? maxRadius = null,
            decimal? minTemperature = null,
            decimal? maxTemperature = null)
        {
            var query = _stars.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                string nameLower = name.ToLower();
                query = query.Where(s => s.CelestialBodyNavigation.Name.ToLower().Contains(nameLower));
            }
            if (spectralClassId.HasValue)
                query = query.Where(s => s.SpectralClassId == spectralClassId.Value);
            if (celestialBodyId.HasValue)
                query = query.Where(s => s.CelestialBodyId == celestialBodyId.Value);
            if (!string.IsNullOrWhiteSpace(constellation))
                query = query.Where(s => s.Constellation != null && s.Constellation.ToLower().Contains(constellation.ToLower()));
            if (!string.IsNullOrWhiteSpace(designation))
                query = query.Where(s => s.Designation != null && s.Designation.ToLower().Contains(designation.ToLower()));
            if (!string.IsNullOrWhiteSpace(bayerDesignation))
                query = query.Where(s => s.BayerDesignation != null && s.BayerDesignation.ToLower().Contains(bayerDesignation.ToLower()));
            if (minApprovalDate.HasValue)
                query = query.Where(s => s.ApprovalDate >= minApprovalDate.Value);
            if (maxApprovalDate.HasValue)
                query = query.Where(s => s.ApprovalDate <= maxApprovalDate.Value);
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
            return await query
                .Include(s => s.CelestialBodyNavigation)
                .Include(s => s.SpectralClassNavigation)
                .ToListAsync();
        }
    }
}