using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class SpectralClassManager : ReadableManager<SpectralClass, int>, ISpectralClassRepository
    {
        public SpectralClassManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<SpectralClass?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(sc => sc.Id == id);
        }

        protected override IQueryable<SpectralClass> WithIncludes(IQueryable<SpectralClass> query)
        {
            return query.Include(sc => sc.Stars);
        }
    }
}