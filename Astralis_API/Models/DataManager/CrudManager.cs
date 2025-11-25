using Microsoft.EntityFrameworkCore;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CrudManager<TEntity, TIdentifier> : ReadableManager<TEntity, TIdentifier>, ICrudRepository<TEntity, TIdentifier> where TEntity : class
    {
        private readonly AstralisDbContext? _context;
        private readonly DbSet<TEntity> _entities;

        public CrudManager(AstralisDbContext context) : base(context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
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