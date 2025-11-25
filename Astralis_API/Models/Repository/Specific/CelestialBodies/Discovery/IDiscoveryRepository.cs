using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IDiscoveryRepository : IDataRepository<Discovery, int, string>
    {
        Task<IEnumerable<Discovery>> GetByCategoryIdAsync(int id);
    }
}