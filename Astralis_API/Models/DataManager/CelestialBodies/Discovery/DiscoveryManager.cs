using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class DiscoveryManager : DataManager<Discovery, int, string>, IDiscoveryRepository
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<Discovery> _discoveries;

        public DiscoveryManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _discoveries = _context.Set<Discovery>();
        }

        protected override IQueryable<Discovery> WithIncludes(IQueryable<Discovery> query)
        {
            return query
                .Include(d => d.AliasStatusNavigation)
                .Include(d => d.ApprovalAliasUserNavigation)
                .Include(d => d.CelestialBodyNavigation)
                .Include(d => d.ApprovalUserNavigation)
                .Include(d => d.UserNavigation)
                .Include(d => d.DiscoveryStatusNavigation);
        }
        public async override Task<IEnumerable<Discovery>> GetByKeyAsync(string reference)
        {
            return await _discoveries.Where(d => d.Title.ToLower().Contains(reference.ToLower()))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Discovery>> SearchAsync(
            string? title = null,
            int? discoveryStatusId = null,
            int? aliasStatusId = null,
            int? discoveryApprovalUserId = null,
            int? aliasApprovalUserId = null)
        {
            var query = _discoveries.AsQueryable();
            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(d => d.Title.ToLower().Contains(title.ToLower()));
            if (discoveryStatusId.HasValue)
                query = query.Where(d => d.DiscoveryStatusId == discoveryStatusId.Value);
            if (aliasStatusId.HasValue)
                query = query.Where(d => d.AliasStatusId == aliasStatusId.Value);
            if (discoveryApprovalUserId.HasValue)
                query = query.Where(d => d.DiscoveryApprovalUserId == discoveryApprovalUserId.Value);
            if (aliasApprovalUserId.HasValue)
                query = query.Where(d => d.AliasApprovalUserId == aliasApprovalUserId.Value);
            return await WithIncludes(query)
                .ToListAsync();
        }
    }
}