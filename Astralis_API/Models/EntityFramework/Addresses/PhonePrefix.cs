using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_phoneprefix_php")]
    public class PhonePrefix
    {
        [Key]
        [Column("php_id")]
        public int Id { get; set; }

        [Column("php_label")]
        [Required(ErrorMessage = "The phone prefix label is required.")]
        [StringLength(7, ErrorMessage = "The phone prefix label cannot be longer than 7 characters.")]
        public string Label { get; set; } = null!;

        [InverseProperty(nameof(Country.PhonePrefixNavigation))]
        public virtual ICollection<Country> Countries { get; set; } = new List<Country>();
        
        [InverseProperty(nameof(User.PhonePrefixNavigation))]
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
