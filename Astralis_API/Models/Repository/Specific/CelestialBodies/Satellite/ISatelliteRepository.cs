using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface ISatelliteRepository : IDataRepository<Satellite, int, string>
    {
        Task<IEnumerable<Satellite>> SearchAsync(
                    string? name = null,
                    int? planetId = null,
                    int? celestialBodyId = null,
                    decimal? minGravity = null,
                    decimal? maxGravity = null,
                    decimal? minRadius = null,
                    decimal? maxRadius = null,
                    decimal? minDensity = null,
                    decimal? maxDensity = null
                );
    }
}