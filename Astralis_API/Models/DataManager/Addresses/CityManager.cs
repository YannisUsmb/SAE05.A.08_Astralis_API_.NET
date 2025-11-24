using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class CityManager : DataManager<City, int, string>
    {
        public CityManager(AstralisDbContext context) : base(context)
        {
        }
    }
}