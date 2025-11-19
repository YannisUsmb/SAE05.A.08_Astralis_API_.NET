using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_userrole_uro")]
    public class UserRole
    {
        [Key]
        [Column("uro_id")]
        public int Id { get; set; }

        [Column("uro_label")]
        [Required(ErrorMessage = "The is required.")]
        [StringLength(50, ErrorMessage = "The label cannot be longer than 50 caracters.")]
        public string Label { get; set; } = null!;

        [InverseProperty(nameof(User.UserRoleNavigation))]
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
