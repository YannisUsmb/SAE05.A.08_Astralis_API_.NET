using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_creditcard_crc")]
    public class CreditCard
    {
        [Key]
        [Column("crc_id")]
        public int Id { get; set; }

        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.CreditCards))]
        public virtual User UserNavigation { get; set; } = null!;
    }
}
