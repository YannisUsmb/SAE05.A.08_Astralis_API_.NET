using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IUserRepository : IDataRepository<User, int, string>
    {
        Task<IEnumerable<User>> GetByUserRoleIdAsync(int id);
    }
}