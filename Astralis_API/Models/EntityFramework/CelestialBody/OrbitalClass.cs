using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_orbitalclass_oct")]
    public class OrbitalClass
    {
        [Key]
        [Column("oct_id")]
        public int IdProduct { get; set; }

        [Required(ErrorMessage = "The label is required")]
        [Column("oct_label", TypeName = "CHAR(3)")]
        public string Label { get; set; } = null!;

        [Column("oct_description")]
        [StringLength(200, ErrorMessage = "The label cannot be longer than 200 characters.")]
        public string? Description { get; set; } = null;
    }
}
