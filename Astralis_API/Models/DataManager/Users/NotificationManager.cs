using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Astralis_API.Models.DataManager
{
    public class NotificationManager : CrudManager<Notification, int>, INotificationRepository
    {
        public NotificationManager(AstralisDbContext context) : base(context)
        {
        }
    }
}