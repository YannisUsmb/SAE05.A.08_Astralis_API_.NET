using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_report_rep")]
    public class Report
    {
        [Key]
        [Column("rep_id")]
        public int Id { get; set; }

        [Column("rst_id")]
        [Required(ErrorMessage = "The report status ID is required.")]
        public int ReportStatusId { get; set; }

        [Column("rem_id")]
        [Required(ErrorMessage = "The report motive ID is required.")]
        public int ReportMotiveId { get; set; }

        [Column("com_id")]
        [Required(ErrorMessage = "The comment ID is required.")]
        public int CommentId { get; set; }

        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [Column("usr_adminid")]
        public int? AdminId { get; set; }

        [Column("rep_description")]
        [Required(ErrorMessage = "The description is required.")]
        [StringLength(150, ErrorMessage = "The description cannot be longer than 150 characters.")]
        public string Description { get; set; } = null!;

        [Column("rep_date")]
        [Required(ErrorMessage = "The date is required.")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ReportMotiveId))]
        [InverseProperty(nameof(ReportMotive.Reports))]
        public virtual ReportMotive ReportMotiveNavigation { get; set; } = null!;

        [ForeignKey(nameof(ReportStatusId))]
        [InverseProperty(nameof(ReportStatus.Reports))]
        public virtual ReportStatus ReportStatusNavigation { get; set; } = null!;

        [ForeignKey(nameof(CommentId))]
        [InverseProperty(nameof(Comment.Reports))]
        public virtual Comment CommentNavigation { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Reports))]
        public virtual User UserNavigation { get; set; } = null!;

        [ForeignKey(nameof(AdminId))]
        [InverseProperty(nameof(User.TreatedReports))]
        public virtual User AdminNavigation { get; set; } = null!;
    }
}