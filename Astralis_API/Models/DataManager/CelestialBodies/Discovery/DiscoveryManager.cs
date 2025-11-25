using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class DiscoveryManager : DataManager<Discovery, int, string>, IDiscoveryRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Discovery> _Discovery;

        public DiscoveryManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _Discovery = _context.Set<Discovery>();
        }

        public async override Task<IEnumerable<Discovery>> GetByKeyAsync(string reference)
        {
            return await _Discovery.Where(d => d.Title.ToLower().Contains(reference.ToLower()))
                            .Include(d => d.AliasStatusNavigation)
                            .Include(d => d.ApprovalAliasUserNavigation)
                            .Include(d => d.CelestialBodyNavigation)
                            .Include(d => d.ApprovalUserNavigation)
                            .Include(d => d.UserNavigation)
                            .Include(d => d.CelestialBodyNavigation)
                            .Include(d => d.DiscoveryStatusNavigation).ToListAsync();
        }

        public async Task<IEnumerable<Discovery>> GetByCategoryIdAsync(int id)
        {
            return await _Discovery.Where(d => d.UserId == id)
                            .Include(d => d.AliasStatusNavigation)
                            .Include(d => d.ApprovalAliasUserNavigation)
                            .Include(d => d.CelestialBodyNavigation)
                            .Include(d => d.ApprovalUserNavigation)
                            .Include(d => d.UserNavigation)
                            .Include(d => d.CelestialBodyNavigation)
                            .Include(d => d.DiscoveryStatusNavigation)
                            .ToListAsync();
        }
    }
}