using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class PhonePrefixManager : ReadableManager<PhonePrefix, int>, IPhonePrefixRepository
    {
        public PhonePrefixManager(AstralisDbContext context) : base(context)
        {
        }
        public new async Task<PhonePrefix?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(pp => pp.Id == id);
        }

        protected override IQueryable<PhonePrefix> WithIncludes(IQueryable<PhonePrefix> query)
        {
            return query
                .Include(p => p.Countries)
                .Include(p=>p.Users);
        }
    }
}