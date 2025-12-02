namespace Astralis_API.Models.Repository
{
    public interface IWriteableRepository<in TEntity>
    {
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entityToUpdate, TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
