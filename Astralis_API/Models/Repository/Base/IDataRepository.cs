namespace Astralis_API.Models.Repository
{
    public interface IDataRepository<TEntity, in TIdentifier, in TKey> : 
        ICrudRepository<TEntity, TIdentifier>,
        IReadableRepository<TEntity, TIdentifier>, 
        IWriteableRepository<TEntity>, 
        ISearchableRepository<TEntity, TKey>
    {
    }
}
