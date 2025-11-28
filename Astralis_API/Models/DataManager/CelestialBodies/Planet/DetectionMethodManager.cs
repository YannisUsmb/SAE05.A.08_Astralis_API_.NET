using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class DetectionMethodManager : CrudManager<DetectionMethod, int>, IDetectionMethodRepository
    {
        public DetectionMethodManager(AstralisDbContext context) : base(context)
        {
        }
    }
}