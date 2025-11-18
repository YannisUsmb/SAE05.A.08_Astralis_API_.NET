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
        [Required(ErrorMessage = "The user role label is required.")]
        [StringLength(50, ErrorMessage = "The user role label cannot be longer than 50 caracters.")]
        public string NameBrand { get; set; } = null!;

        [InverseProperty(nameof(User.UserRoleId))]
        public virtual ICollection<User> Users { get; set; } = null!;
    }
}
