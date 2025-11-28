using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Astralis_API.Models.DataManager
{
    public class UserManager : DataManager<User, int, string>, IUserRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<User> _users;

        public UserManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _users = _context.Set<User>();
        }

        public async override Task<IEnumerable<User>> GetByKeyAsync(string username)
        {
            return await _users.Where(u => u.Username.ToLower().Contains(username.ToLower()))
                            .Include(u => u.DeliveryAddressNavigation)
                            .Include(u => u.InvoicingAddressNavigation)
                            .Include(u => u.PhonePrefixNavigation)
                            .Include(u => u.UserRoleNavigation)
                            .Include(u => u.CreditCards)
                            .Include(u => u.Events)
                            .Include(u => u.Commands)
                            .Include(u => u.CartItems)
                            .Include(u => u.Products)
                            .Include(u => u.EventInterests)
                            .Include(u => u.Articles)
                            .Include(u => u.ArticleInterests)
                            .Include(u => u.Comments)
                            .Include(u => u.Reports)
                            .Include(u => u.TreatedReports)
                            .Include(u => u.Discoveries)
                            .Include(u => u.ApprovedDiscoveries)
                            .Include(u => u.ApprovedAliasDiscoveries)
                            .Include(u => u.UserNotifications) 
                            .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByUserRoleIdAsync(int id)
        {
            return await _users.Where(u => u.UserRoleId == id)
                            .Include(u => u.DeliveryAddressNavigation)
                            .Include(u => u.InvoicingAddressNavigation)
                            .Include(u => u.PhonePrefixNavigation)
                            .Include(u => u.UserRoleNavigation)
                            .Include(u => u.CreditCards)
                            .Include(u => u.Events)
                            .Include(u => u.Commands)
                            .Include(u => u.CartItems)
                            .Include(u => u.Products)
                            .Include(u => u.EventInterests)
                            .Include(u => u.Articles)
                            .Include(u => u.ArticleInterests)
                            .Include(u => u.Comments)
                            .Include(u => u.Reports)
                            .Include(u => u.TreatedReports)
                            .Include(u => u.Discoveries)
                            .Include(u => u.ApprovedDiscoveries)
                            .Include(u => u.ApprovedAliasDiscoveries)
                            .Include(u => u.UserNotifications)
                            .ToListAsync();
        }
    }
}
