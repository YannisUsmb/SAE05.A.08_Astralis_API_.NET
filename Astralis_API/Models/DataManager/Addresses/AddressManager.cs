using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class AddressManager : CrudManager<Address, int>, IAddressRepository
    {
        public AddressManager(AstralisDbContext context) : base(context)
        {
        }
        public override async Task<Address?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(a => a.Id == id);
        }
        protected override IQueryable<Address> WithIncludes(IQueryable<Address> query)
        {
            return query
                .Include(a => a.CityNavigation)
                    .ThenInclude(c => c.CountryNavigation)
                .Include(a => a.InvoicingAddressUsers)
                .Include(a => a.DeliveryAddressUsers);
        }
    }
}