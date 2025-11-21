using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_star_sta")]
    public class Star
    {
        [Key]
        [Column("sta_id")]
        public int Id { get; set; }

        [Column("ceb_id")]
        [Required(ErrorMessage = "The celestial body ID is required.")]
        public int CelestialBodyId { get; set; }

        [Column("spc_id")]
        public int? SpectralClassId { get; set; }

        [Column("sta_designation")]
        [StringLength(10, ErrorMessage = "The designation cannot be longer than 10 characters.")]
        public string? Designation { get; set; }

        [Column("sta_approvaldate")]
        public DateOnly? ApprovalDate { get; set; }

        [Column("sta_constellation")]
        [StringLength(25, ErrorMessage = "The constellation cannot be longer than 25 characters.")]
        public string? Constellation { get; set; }

        [Column("sta_bayerdesignation")]
        [StringLength(30, ErrorMessage = "The bayer designation cannot be longer than 30 characters.")]
        public string? BayerDesignation { get; set; }

        [Column("sta_distance", TypeName = "NUMERIC(20,15)")]
        public decimal? Distance { get; set; }

        [Column("sta_luminosity", TypeName = "NUMERIC(30,20)")]
        public decimal? Luminosity { get; set; }

        [Column("sta_radius", TypeName = "NUMERIC(20,15)")]
        public decimal? Radius { get; set; }

        [Column("sta_temperature", TypeName = "NUMERIC(20,15)")]
        public decimal? Temperature { get; set; }

        [ForeignKey(nameof(CelestialBodyId))]
        [InverseProperty(nameof(CelestialBody.StarNavigation))]
        public virtual CelestialBody CelestialBodyNavigation { get; set; } = null!;

        [ForeignKey(nameof(SpectralClassId))]
        [InverseProperty(nameof(SpectralClassNavigation.Stars))]
        public virtual SpectralClass? SpectralClassNavigation { get; set; }
    }
}