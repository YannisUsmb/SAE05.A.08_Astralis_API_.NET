using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class UserNotificationTypeManager : JoinManager<UserNotificationType, int, int>, IUserNotificationTypeRepository
    {
        public UserNotificationTypeManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<UserNotificationType?> GetByIdAsync(int userId, int notificationTypeId)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(un => un.UserId == userId && un.NotificationTypeId == notificationTypeId);
        }
        public async Task<IEnumerable<UserNotificationType?>> GetByUserIdAsync(int userId)
        {
            return await WithIncludes(_entities.Where(un => un.UserId == userId)).ToListAsync();
        }

        protected override IQueryable<UserNotificationType> WithIncludes(IQueryable<UserNotificationType> query)
        {
            return query.Include(un => un.UserNavigation)
                        .Include(un => un.NotificationTypeNavigation);
        }
    }
}