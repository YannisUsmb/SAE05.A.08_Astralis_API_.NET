using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class GalaxyQuasarManager : DataManager<GalaxyQuasar, int, string>, IGalaxyQuasarRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<GalaxyQuasar> _galaxyQuasars;

        public GalaxyQuasarManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _galaxyQuasars = _context.Set<GalaxyQuasar>();
        }

        protected override IQueryable<GalaxyQuasar> WithIncludes(IQueryable<GalaxyQuasar> query)
        {
            return query.Include(gq => gq.CelestialBodyNavigation)
                        .Include(gq => gq.GalaxyQuasarClassNavigation);
        }

        public async override Task<IEnumerable<GalaxyQuasar>> GetByKeyAsync(string reference)
        {
            return await WithIncludes(_galaxyQuasars.Where(gq => gq.Reference.ToLower().Contains(reference.ToLower()))
                            ).ToListAsync();
        }

        public async Task<IEnumerable<GalaxyQuasar>> SearchAsync(
            string? reference = null,
            int? celestialBodyId = null,
            int? galaxyQuasarClassId = null,
            decimal? minRightAscension = null,
            decimal? maxRightAscension = null,
            decimal? minDeclination = null,
            decimal? maxDeclination = null,
            decimal? minRedshift = null,
            decimal? maxRedshift = null,
            decimal? minRMagnitude = null,
            decimal? maxRMagnitude = null,
            int? minMjdObs = null,
            int? maxMjdObs = null)
        {
            var query = _galaxyQuasars.AsQueryable();
            if (!string.IsNullOrWhiteSpace(reference))
            {
                string refLower = reference.ToLower();
                query = query.Where(gq => gq.Reference != null && gq.Reference.ToLower().Contains(refLower));
            }
            if (celestialBodyId.HasValue)
                query = query.Where(gq => gq.CelestialBodyId == celestialBodyId.Value);
            if (galaxyQuasarClassId.HasValue)
                query = query.Where(gq => gq.GalaxyQuasarClassId == galaxyQuasarClassId.Value);
            if (minRightAscension.HasValue)
                query = query.Where(gq => gq.RightAscension >= minRightAscension.Value);
            if (maxRightAscension.HasValue)
                query = query.Where(gq => gq.RightAscension <= maxRightAscension.Value);
            if (minDeclination.HasValue)
                query = query.Where(gq => gq.Declination >= minDeclination.Value);
            if (maxDeclination.HasValue)
                query = query.Where(gq => gq.Declination <= maxDeclination.Value);
            if (minRedshift.HasValue)
                query = query.Where(gq => gq.Redshift >= minRedshift.Value);
            if (maxRedshift.HasValue)
                query = query.Where(gq => gq.Redshift <= maxRedshift.Value);
            if (minRMagnitude.HasValue)
                query = query.Where(gq => gq.RMagnitude >= minRMagnitude.Value);
            if (maxRMagnitude.HasValue)
                query = query.Where(gq => gq.RMagnitude <= maxRMagnitude.Value);
            if (minMjdObs.HasValue)
                query = query.Where(gq => gq.ModifiedJulianDateObservation >= minMjdObs.Value);
            if (maxMjdObs.HasValue)
                query = query.Where(gq => gq.ModifiedJulianDateObservation <= maxMjdObs.Value);
            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}