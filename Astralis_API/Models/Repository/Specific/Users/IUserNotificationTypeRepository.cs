using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IUserNotificationTypeRepository : IJoinRepository<UserNotificationType, int, int>
    {
        Task<IEnumerable<UserNotificationType?>> GetByUserIdAsync(int userId);
    }
}