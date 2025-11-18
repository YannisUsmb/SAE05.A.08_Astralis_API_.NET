using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_asteroid_ast")]
    public class Asteroid
    {
        [Key]
        [Column("ast_id")]
        public int Id { get; set; }

        [Column("ceb_id")]
        [Required(ErrorMessage = "The celestial body id is required")]
        public int CelestialBodyId { get; set; }

        [Column("oct_id")]
        [Required(ErrorMessage = "The orbital class id is required")]
        public int OrbitalClassId { get; set; }

        [Column("ast_reference")]
        [StringLength(20, ErrorMessage = "The reference cannot be longer than 20 characters.")]
        public string? Reference { get; set; }

        [Column("ast_absolutemagnitude", TypeName = "NUMERIC(4,2)")]
        public decimal? AbsoluteMagnitude { get; set; }

        [Column("ast_diameterminkm", TypeName = "NUMERIC(14,12)")]
        public decimal? DiameterMinKm { get; set; }

        [Column("ast_diametermaxkm", TypeName = "NUMERIC(14,12)")]
        public decimal? DiameterMaxKm { get; set; }

        [Column("ast_ispotentiallyhazardous")]
        public bool? IsPotentiallyHazardous { get; set; }

        [Column("ast_orbitid")]
        public int? OrbitId { get; set; }

        [Column("ast_orbitdeterminationdate")]
        public DateTime? OrbitDeterminationDate { get; set; }

        [Column("ast_firstobservationdate")]
        public DateTime? FirstObservationDate { get; set; }

        [Column("ast_lastobservationdate")]
        public DateTime? LastObservationDate { get; set; }

        [Column("ast_semimajoraxis", TypeName = "NUMERIC(14,12)")]
        public decimal? SemiMajorAxis { get; set; }

        [Column("ast_inclination", TypeName = "NUMERIC(15,12)")]
        public decimal? Inclination { get; set; }


        [ForeignKey(nameof(OrbitalClassId))]
        [InverseProperty(nameof(OrbitalClassNavigation.Asteroids))]
        public virtual OrbitalClass OrbitalClassNavigation { get; set; } = null!;
    }

}
