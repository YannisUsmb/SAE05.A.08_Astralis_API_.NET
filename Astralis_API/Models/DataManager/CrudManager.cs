using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;

namespace Astralis_API.Models.DataManager
{
    public class CrudManager<TEntity, TIdentifier> : ReadableManager<TEntity, TIdentifier>, ICrudRepository<TEntity, TIdentifier> where TEntity : class
    {
        public CrudManager(AstralisDbContext context) : base(context) { }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
        {
            _entities.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}