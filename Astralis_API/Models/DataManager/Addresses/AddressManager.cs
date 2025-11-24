using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class AddressManager : DataManager<Address, int, string>
    {
        public AddressManager(AstralisDbContext context) : base(context)
        {
        }
    }
}