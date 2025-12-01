using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IStarRepository : IDataRepository<Star, int, string>
    {
        Task<IEnumerable<Star>> SearchAsync(
                    string? name = null,
                    IEnumerable<int>? spectralClassIds = null,
                    string? constellation = null,
                    string? designation = null,
                    string? bayerDesignation = null,
                    decimal? minDistance = null,
                    decimal? maxDistance = null,
                    decimal? minLuminosity = null,
                    decimal? maxLuminosity = null,
                    decimal? minRadius = null,
                    decimal? maxRadius = null,
                    decimal? minTemperature = null,
                    decimal? maxTemperature = null
                );
    }
}