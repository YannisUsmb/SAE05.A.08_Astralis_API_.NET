using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class DetectionMethodManager : CrudManager<DetectionMethod, int>, IDetectionMethodRepository
    {
        public DetectionMethodManager(AstralisDbContext context) : base(context)
        {
        }
        protected override IQueryable<DetectionMethod> WithIncludes(IQueryable<DetectionMethod> query)
        {
            return query.Include(dm => dm.Planets);
        }
    }
}