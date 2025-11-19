namespace Astralis_API.Models.Repository
{
    public interface ISearchRepository<TEntity, TKey>
    {
        Task<TEntity> GetByKeyAsync(TKey key);
    }
}
