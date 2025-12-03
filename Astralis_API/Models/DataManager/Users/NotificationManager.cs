using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class NotificationManager : CrudManager<Notification, int>, INotificationRepository
    {
        public NotificationManager(AstralisDbContext context) : base(context)
        {
        }

        public override async Task<Notification?> GetByIdAsync(int id)
        {
            return await WithIncludes(_entities)
                         .FirstOrDefaultAsync(n => n.Id == id);
        }

        protected override IQueryable<Notification> WithIncludes(IQueryable<Notification> query)
        {
            return query.Include(n => n.UserNotifications)
                        .Include(n => n.NotificationTypeNavigation);
        }
    }
}