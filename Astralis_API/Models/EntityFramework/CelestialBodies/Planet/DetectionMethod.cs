using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_detectionmethod_dem")]
    public class DetectionMethod
    {
        [Key]
        [Column("dem_id")]
        public int Id { get; set; }

        [Column("dem_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(20, ErrorMessage = "The label cannot be longer than 20 caracters.")]
        public string Label { get; set; } = null!;

        [Column("dem_description")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 caracters.")]
        public string? Description { get; set; }

        [InverseProperty(nameof(Planet.DetectionMethodNavigation))]
        public virtual ICollection<Planet> Planets { get; set; } = new List<Planet>();
    }
}