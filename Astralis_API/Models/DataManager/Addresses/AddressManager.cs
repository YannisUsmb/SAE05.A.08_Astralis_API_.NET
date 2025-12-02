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