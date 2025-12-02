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

        public override async Task<UserRole?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(ur => ur.Id == id);
        }

        protected override IQueryable<UserRole> WithIncludes(IQueryable<UserRole> query)
        {
            return query.Include(ur => ur.Users);
        }
    }
}