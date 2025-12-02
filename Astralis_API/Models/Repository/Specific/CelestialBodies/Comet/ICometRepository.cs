using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface ICometRepository : IDataRepository<Comet, int, string>
    {
        Task<IEnumerable<Comet>> SearchAsync(
            string? reference = null,
            decimal? minEccentricity = null,
            decimal? maxEccentricity = null,
            decimal? minInclination = null,
            decimal? maxInclination = null,
            decimal? minPerihelionAU = null,
            decimal? maxPerihelionAU = null,
            decimal? minAphelionAU = null,
            decimal? maxAphelionAU = null,
            decimal? minOrbitalPeriod = null,
            decimal? maxOrbitalPeriod = null,
            decimal? minMOID = null,
            decimal? maxMOID = null
        );
    }
}