using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class UserRoleManager : ReadableManager<UserRole, int>, IUserRoleRepository
    {
        public UserRoleManager(AstralisDbContext context) : base(context)
        {
        }
    }
}