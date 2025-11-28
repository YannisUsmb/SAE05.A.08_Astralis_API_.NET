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
        protected override IQueryable<Notification> WithIncludes(IQueryable<Notification> query)
        {
            return query.Include(n => n.UserNotifications);
        }
    }
}