using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IStarRepository : IDataRepository<Star, int, string>
    {
        Task<IEnumerable<Star>> GetBySpectralClassIdAsync(int id);
    }
}