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

        [Required(ErrorMessage = "The planet type id is required")]
        [Column("plt_id")]
        public int PlanetTypeId { get; set; }

        [Required(ErrorMessage = "The detection method id is required")]
        [Column("dem_id")]
        public int DetectionMethodId { get; set; }

        [Required(ErrorMessage = "The celestial body id is required")]
        [Column("ceb_id")]
        public int CelestialBodyId { get; set; }

        [Column("pla_distance")]
        [StringLength(10)]
        public string? Distance { get; set; }

        [Column("pla_discoveryyear")]
        public DateTime? DiscoveryYear { get; set; }

        [Column("pla_mass")]
        [StringLength(20)]
        public string? Mass { get; set; }

        [Column("pla_radius")]
        [StringLength(10)]
        public string? Radius { get; set; }

        [Column("pla_temperature")]
        [StringLength(15)]
        public string? Temperature { get; set; }

        [Column("pla_orbitalperiod", TypeName = "NUMERIC(14,12)")]
        public decimal? OrbitalPeriod { get; set; }

        [Column("pla_eccentricity", TypeName = "NUMERIC(4,3)")]
        public decimal? Eccentricity { get; set; }

        [Column("pla_stellarmagnitude", TypeName = "NUMERIC(5,3)")]
        public decimal? StellarMagnitude { get; set; }

        [Column("pla_hoststartemperature")]
        [StringLength(15)]
        public string? HostStarTemperature { get; set; }

        [Column("pla_hoststarmass")]
        [StringLength(15)]
        public string? HostStarMass { get; set; }

        [Column("pla_remark")]
        [StringLength(250)]
        public string? Remark { get; set; }


        [ForeignKey(nameof(PlanetTypeId))]
        [InverseProperty(nameof(PlanetType.Planets))]
        public virtual PlanetType? PlanetTypeNavigation { get; set; }

        [ForeignKey(nameof(DetectionMethodId))]
        [InverseProperty(nameof(DetectionMethod.Planets))]
        public virtual DetectionMethod? DetectionMethodNavigation { get; set; }
    }
}
