using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ReadableManager<TEntity, TIdentifier> : IReadableRepository<TEntity, TIdentifier> where TEntity : class
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<TEntity> _entities;

        public ReadableManager(AstralisDbContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TIdentifier id)
        {
            return await _entities.FindAsync(id);
        }
    }
}
