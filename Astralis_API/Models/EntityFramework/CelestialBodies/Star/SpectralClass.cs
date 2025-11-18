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
        [Required(ErrorMessage = "The label is required")]
        [StringLength(10, ErrorMessage = "The label cannot be longer than 10 characters.")]
        public string Label { get; set; } = null!;


        [InverseProperty(nameof(Star.SpectralClassNavigation))]
        public virtual ICollection<Star> Stars { get; set; } = new List<Star>();
    }
}
