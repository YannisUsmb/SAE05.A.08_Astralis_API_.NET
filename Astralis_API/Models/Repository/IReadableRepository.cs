namespace Astralis_API.Models.Repository
{
    public interface IReadableRepository<TEntity, TIdentifier>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TIdentifier id);
    }
}
