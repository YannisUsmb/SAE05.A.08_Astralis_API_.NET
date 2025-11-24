using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class CountryManager : ReadableManager<Country, int>
    {
        public CountryManager(AstralisDbContext context) : base(context)
        {
        }
    }
}