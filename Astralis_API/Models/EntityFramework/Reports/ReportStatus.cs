using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_reportstatus_rst")]
    public class ReportStatus
    {
        [Key]
        [Column("rst_id")]
        public int Id { get; set; }

        [Column("rst_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(30, ErrorMessage = "The label cannot be longer than 30 characters.")]
        public string Label { get; set; } = null!;

        [InverseProperty(nameof(Report.ReportStatusNavigation))]
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    }
}