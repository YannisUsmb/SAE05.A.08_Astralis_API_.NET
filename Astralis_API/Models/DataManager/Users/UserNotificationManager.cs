using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class UserNotificationManager : CrudManager<UserNotification, int>, IUserNotificationRepository
    {
        public UserNotificationManager(AstralisDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(int userId)
        {
            return await WithIncludes(_entities)
                         .Where(un => un.UserId == userId)
                         .OrderByDescending(un => un.ReceivedAt)
                         .ToListAsync();
        }

        protected override IQueryable<UserNotification> WithIncludes(IQueryable<UserNotification> query)
        {
            return query.Include(un => un.UserNavigation)
                        .Include(un => un.NotificationNavigation)
                            .ThenInclude(n => n.NotificationTypeNavigation);
        }
    }
}