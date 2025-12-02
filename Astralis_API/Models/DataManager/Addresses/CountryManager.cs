using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class CountryManager : ReadableManager<Country, int>, ICountryRepository
    {
        public CountryManager(AstralisDbContext context) : base(context)
        {
        }
        public override async Task<Country?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(c => c.Id == id);
        }

        protected override IQueryable<Country> WithIncludes(IQueryable<Country> query)
        {
            return query
                .Include(c => c.PhonePrefixNavigation)
                .Include(c => c.Cities);
        }
    }
}