using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CityManager : CrudManager<City, int>, ICityRepository
    {
        public CityManager(AstralisDbContext context) : base(context)
        {
        }
        protected override IQueryable<City> WithIncludes(IQueryable<City> query)
        {
            return query
                .Include(c => c.CountryNavigation)
                .Include(c => c.Addresses);
        }
    }
}