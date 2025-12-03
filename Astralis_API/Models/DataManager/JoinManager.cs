using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.DataManager
{
    public class JoinManager<TEntity, TKey1, TKey2> : IJoinRepository<TEntity, TKey1, TKey2>
        where TEntity : class
    {
        protected readonly AstralisDbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public JoinManager(AstralisDbContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await WithIncludes(_entities).ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey1 key1, TKey2 key2)
        {
            return await _entities.FindAsync(key1, key2);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
        {
            _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        protected virtual IQueryable<TEntity> WithIncludes(IQueryable<TEntity> query)
        {
            return query;
        }
    }
}