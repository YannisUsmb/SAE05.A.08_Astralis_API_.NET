using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_spectralclass_spc")]
    public class SpectralClass
    {
        [Key]
        [Column("spc_id")]
        public int Id { get; set; }

        [Column("spc_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(10, ErrorMessage = "The label cannot be longer than 10 caracters.")]
        public string Label { get; set; } = null!;

        [Column("spc_description")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 caracters.")]
        public string? Description { get; set; }

        [InverseProperty(nameof(Star.SpectralClassNavigation))]
        public virtual ICollection<Star> Stars { get; set; } = new List<Star>();
    }
}