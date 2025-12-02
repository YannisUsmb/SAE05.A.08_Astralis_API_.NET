using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class DetectionMethodManager : ReadableManager<DetectionMethod, int>, IDetectionMethodRepository
    {
        public DetectionMethodManager(AstralisDbContext context) : base(context)
        {
        }

        public new async Task<DetectionMethod?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(cb => cb.Id == id);
        }

        protected override IQueryable<DetectionMethod> WithIncludes(IQueryable<DetectionMethod> query)
        {
            return query.Include(dm => dm.Planets);
        }
    }
}