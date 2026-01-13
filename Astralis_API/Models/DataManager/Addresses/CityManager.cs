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
        public override async Task<City?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(c => c.Id == id);
        }

        protected override IQueryable<City> WithIncludes(IQueryable<City> query)
        {
            return query
                .Include(c => c.CountryNavigation)
                .Include(c => c.Addresses);
        }
        public async Task<IEnumerable<City>> SearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return new List<City>();

            term = term.Trim().ToLower();

            return await _context.Cities
                .Where(c => c.Name.ToLower().StartsWith(term) || c.PostCode.ToLower().StartsWith(term))
                .OrderByDescending(c => c.PostCode.ToLower() == term)
                .ThenByDescending(c => c.Name.ToLower() == term)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }
    }
}