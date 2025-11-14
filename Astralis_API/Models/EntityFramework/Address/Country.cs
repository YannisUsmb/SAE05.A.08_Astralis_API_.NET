using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_country_cou")]
    public class Country
    {
        [Key]
        [Column("cou_id")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "The phone prefix ID is required.")]
        [Column("php_id")]
        public int PhonePrefixId { get; set; }

        [Column("cou_name")]
        [Required(ErrorMessage = "The country name is required.")]
        [StringLength(80, ErrorMessage = "The country name cannot be longer than 80 characters.")]
        public String Name { get; set; }

        [ForeignKey(nameof(PhonePrefixId))]
        [InverseProperty(nameof(PhonePrefix.Countries))]
        public virtual PhonePrefix? PhonePrefix { get; set; }
        
        [InverseProperty(nameof(City.Country))]
        public virtual ICollection<Country> Cities { get; set; } = null!;
    }
}
