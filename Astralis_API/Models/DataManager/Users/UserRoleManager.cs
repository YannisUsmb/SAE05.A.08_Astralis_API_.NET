using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class UserRoleManager : ReadableManager<UserRole, int>, IUserRoleRepository
    {
        public UserRoleManager(AstralisDbContext context) : base(context)
        {
        }

        protected override IQueryable<UserRole> WithIncludes(IQueryable<UserRole> query)
        {
            return query.Include(ur => ur.Users);
        }
    }
}