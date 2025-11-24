using Microsoft.EntityFrameworkCore;
using Astralis_API.Models.EntityFramework;
namespace Astralis_API.Models.DataManager
{
    public class DataManager<TEntity, TIdentifier, TKey> where TEntity : class
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<TEntity> _entities;

        public DataManager(AstralisDbContext context)
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

        public async Task AddAsync(TEntity entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
        {
            _entities.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}