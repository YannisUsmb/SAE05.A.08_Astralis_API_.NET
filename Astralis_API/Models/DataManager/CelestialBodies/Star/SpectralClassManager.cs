using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class SpectralClassManager : CrudManager<SpectralClass, int>, ISpectralClassRepository
    {
        public SpectralClassManager(AstralisDbContext context) : base(context)
        {
        }
    }
}