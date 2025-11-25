using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CountryManager : CrudManager<Country, int>, ICountryRepository
    {
        public CountryManager(AstralisDbContext context) : base(context)
        {
        }
    }
}