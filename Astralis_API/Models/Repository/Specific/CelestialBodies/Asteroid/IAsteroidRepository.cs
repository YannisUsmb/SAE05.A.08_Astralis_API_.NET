using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Repository
{
    public interface IAsteroidRepository : IDataRepository<Asteroid, int, string>
    {
        Task<IEnumerable<Asteroid>> SearchAsync(
            string? reference = null,
            IEnumerable<int>? orbitalClassIds = null,
            bool? isPotentiallyHazardous = null,
            int? orbitId = null,
            decimal? absoluteMagnitude = null,
            decimal? minDiameter = null,
            decimal? maxDiameter = null,
            decimal? minInclination = null,
            decimal? maxInclination = null,
            decimal? maxSemiMajorAxis = null,
            decimal? minSemiMajorAxis = null,
            DateTime? firstObservationDate = null,
            DateTime? lastObservationDate = null
        );
    }
}