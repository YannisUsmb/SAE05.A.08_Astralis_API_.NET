namespace Astralis_API.Models.Repository
{
    public interface IJoinRepository<TEntity, in TKey1, in TKey2>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TKey1 key1, TKey2 key2);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entityToUpdate, TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}