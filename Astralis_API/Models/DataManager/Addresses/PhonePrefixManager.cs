using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.DataManager
{
    public class PhonePrefixManager : ReadableManager<PhonePrefix, int>
    {
        public PhonePrefixManager(AstralisDbContext context) : base(context)
        {
        }
    }
}