namespace Astralis_API.Models.Repository
{
    public interface IDataRepository<TEntity, in TIdentifier, in TKey> : 
        IReadableRepository<TEntity, TIdentifier>, 
        IWriteableRepository<TEntity>, 
        ISearchableRepository<TEntity, TKey>
    {
    }
}
