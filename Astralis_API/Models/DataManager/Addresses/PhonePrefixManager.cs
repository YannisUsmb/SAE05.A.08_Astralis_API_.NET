using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class PhonePrefixManager : ReadableManager<PhonePrefix, int>, IPhonePrefixRepository
    {
        public PhonePrefixManager(AstralisDbContext context) : base(context)
        {
        }
    }
}