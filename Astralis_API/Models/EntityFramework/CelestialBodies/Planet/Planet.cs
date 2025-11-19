using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_planet_pla")]
    public class Planet
    {
        [Key]
        [Column("pla_id")]
        public int Id { get; set; }

        [Column("ceb_id")]
        [Required(ErrorMessage = "The celestial body ID is required")]
        public int CelestialBodyId { get; set; }

        [Column("plt_id")]
        [Required(ErrorMessage = "The planet type ID is required")]
        public int PlanetTypeId { get; set; }

        [Column("dem_id")]
        [Required(ErrorMessage = "The detection method ID is required")]
        public int DetectionMethodId { get; set; }

        [Column("pla_distance")]
        [StringLength(10, ErrorMessage = "The distance cannot be longer than 10 caracters.")]
        public string? Distance { get; set; }

        [Column("pla_discoveryyear")]
        public DateTime? DiscoveryYear { get; set; }

        [Column("pla_mass")]
        [StringLength(20, ErrorMessage = "The mass cannot be longer than 20 caracters.")]
        public string? Mass { get; set; }

        [Column("pla_radius")]
        [StringLength(10, ErrorMessage = "The radius cannot be longer than 10 caracters.")]
        public string? Radius { get; set; }

        [Column("pla_temperature")]
        [StringLength(15, ErrorMessage = "The temperature cannot be longer than 15 caracters.")]
        public string? Temperature { get; set; }

        [Column("pla_orbitalperiod", TypeName = "NUMERIC(14,12)")]
        public decimal? OrbitalPeriod { get; set; }

        [Column("pla_eccentricity", TypeName = "NUMERIC(4,3)")]
        public decimal? Eccentricity { get; set; }

        [Column("pla_stellarmagnitude", TypeName = "NUMERIC(5,3)")]
        public decimal? StellarMagnitude { get; set; }

        [Column("pla_hoststartemperature")]
        [StringLength(15, ErrorMessage = "The host star temperature cannot be longer than 15 caracters.")]
        public string? HostStarTemperature { get; set; }

        [Column("pla_hoststarmass")]
        [StringLength(15, ErrorMessage = "The host stat mass cannot be longer than 15 caracters.")]
        public string? HostStarMass { get; set; }

        [Column("pla_remark")]
        [StringLength(250, ErrorMessage = "The remark cannot be longer than 250 caracters.")]
        public string? Remark { get; set; }

        [ForeignKey(nameof(CelestialBodyId))]
        [InverseProperty(nameof(CelestialBody.PlanetNavigation))]
        public virtual CelestialBody CelestialBodyNavigation { get; set; } = null!;

        [ForeignKey(nameof(PlanetTypeId))]
        [InverseProperty(nameof(PlanetType.Planets))]
        public virtual PlanetType PlanetTypeNavigation { get; set; } = null!;

        [ForeignKey(nameof(DetectionMethodId))]
        [InverseProperty(nameof(DetectionMethod.Planets))]
        public virtual DetectionMethod DetectionMethodNavigation { get; set; } = null!;

        [InverseProperty(nameof(Satellite.PlanetNavigation))]
        public virtual ICollection<Satellite> Satellites { get; set; } = new List<Satellite>();
    }
}
