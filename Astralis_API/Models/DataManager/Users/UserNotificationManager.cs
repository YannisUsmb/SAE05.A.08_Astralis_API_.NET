using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class UserNotificationManager : JoinManager<UserNotification, int, int>, IUserNotificationRepository
    {
        public UserNotificationManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<UserNotification?> GetByIdAsync(int userId, int notificationId)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(un => un.UserId == userId && un.NotificationId == notificationId);
        }

        protected override IQueryable<UserNotification> WithIncludes(IQueryable<UserNotification> query)
        {
            return query.Include(un => un.UserNavigation)
                .Include(un => un.NotificationNavigation);
        }
    }
}