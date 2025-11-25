using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CityManager : CrudManager<City, int>, ICityRepository
    {
        public CityManager(AstralisDbContext context) : base(context)
        {
        }
    }
}