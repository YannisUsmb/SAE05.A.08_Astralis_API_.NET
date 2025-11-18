using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_command_cmd")]
    public class Command
    {
        [Key]
        [Column("cmd_id")]
        public int Id { get; set; }

        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [Column("cos_id")]
        [Required(ErrorMessage = "The command status ID is required.")]
        public int CommandStatusId { get; set; }

        [Column("cmd_date")]
        [Required(ErrorMessage = "The date is required.")]
        public DateTime Date { get; set; }

        [Column("cmd_total")]
        [Required(ErrorMessage = "The total amount is required.")]
        [DataType(DataType.Currency)]
        [Range(0, 999999.99, ErrorMessage = "Total must be positive.")]
        public decimal Total { get; set; }

        [Column("cmd_pdfname")]
        [Required(ErrorMessage = "The name of the PDF is required.")]
        [StringLength(255, ErrorMessage = "The name of the PDF cannot be longer than 255 characters.")]
        public string PdfName { get; set; } = null!;

        [Column("cmd_pdfpath")]
        [Required(ErrorMessage = "The path of the PDF is required.")]
        [StringLength(255, ErrorMessage = "The path of the PDF cannot be longer than 255 characters.")]
        public string PdfPath { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Commands))]
        public virtual User UserNavigation { get; set; } = null!;

        [ForeignKey(nameof(CommandStatusId))]
        [InverseProperty(nameof(CommandStatus.Commands))]
        public virtual CommandStatus CommandStatusNavigation { get; set; } = null!;

        [InverseProperty(nameof(OrderDetail.CommandNavigation))]
        public virtual ICollection<OrderDetail> OrderDetailsNavigation { get; set; } = new List<OrderDetail>();
    }
}
