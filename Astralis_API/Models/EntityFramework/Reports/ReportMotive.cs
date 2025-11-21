using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_reportmotive_rem")]
    public class ReportMotive
    {
        [Key]
        [Column("rem_id")]
        public int Id { get; set; }

        [Column("rem_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(100, ErrorMessage = "The label cannot be longer than 100 characters.")]
        public string Label { get; set; } = null!;

        [Column("rem_description")]
        [StringLength(300, ErrorMessage = "The descriptoin cannot be longer than 100 characters.")]
        public string? Description { get; set; }

        [InverseProperty(nameof(Report.ReportMotiveNavigation))]
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}