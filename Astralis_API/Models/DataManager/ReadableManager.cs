using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class ReadableManager<TEntity, TIdentifier> : IReadableRepository<TEntity, TIdentifier> where TEntity : class
    {
        protected readonly AstralisDbContext? _context;
        protected readonly DbSet<TEntity> _entities;

        public ReadableManager(AstralisDbContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await WithIncludes(_entities).ToListAsync();
        }


        public async Task<TEntity> GetByIdAsync(TIdentifier id)
        {
            return await _entities.FindAsync(id);
        }

        protected virtual IQueryable<TEntity> WithIncludes(IQueryable<TEntity> query)
        {
            return query;
        }
    }
}
