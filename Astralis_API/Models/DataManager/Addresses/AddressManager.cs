using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Models.DataManager;
namespace Astralis_API.Models.DataManager
{
    public class AddressManager : CrudManager<Address, int>, IAddressRepository
    {
        public AddressManager(AstralisDbContext context) : base(context)
        {
        }
    }
}