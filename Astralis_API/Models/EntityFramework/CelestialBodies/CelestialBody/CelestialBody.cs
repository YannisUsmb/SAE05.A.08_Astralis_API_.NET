using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_celestialbody_ceb")]
    public class CelestialBody
    {
        [Key]
        [Column("ceb_id")]
        public int Id { get; set; }

        [Column("cbt_id")]
        [Required(ErrorMessage = "The celestial body type ID is required.")]
        public int CelestialBodyTypeId { get; set; }

        [Column("ceb_name")]
        [Required(ErrorMessage = "The name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot be longer than 100 caracters.")]
        public string Name { get; set; } = null!;

        [Column("ceb_alias")]
        [StringLength(100, ErrorMessage = "The alias cannot be longer than 100 caracters.")]
        public string? Alias { get; set; }

        [ForeignKey(nameof(CelestialBodyTypeId))]
        [InverseProperty(nameof(CelestialBodyType.CelestialBodies))]
        public virtual CelestialBodyType CelestialBodyTypeNavigation { get; set; } = null!;

        [InverseProperty(nameof(Planet.CelestialBodyNavigation))]
        public virtual Planet? PlanetNavigation { get; set; }

        [InverseProperty(nameof(Star.CelestialBodyNavigation))]
        public virtual Star? StarNavigation { get; set; }

        [InverseProperty(nameof(Satellite.CelestialBodyNavigation))]
        public virtual Satellite? SatelliteNavigation { get; set; }

        [InverseProperty(nameof(Asteroid.CelestialBodyNavigation))]
        public virtual Asteroid? AsteroidNavigation { get; set; }

        [InverseProperty(nameof(Comet.CelestialBodyNavigation))]
        public virtual Comet? CometNavigation { get; set; }

        [InverseProperty(nameof(GalaxyQuasar.CelestialBodyNavigation))]
        public virtual GalaxyQuasar? GalaxyQuasarNavigation { get; set; }
    }
}