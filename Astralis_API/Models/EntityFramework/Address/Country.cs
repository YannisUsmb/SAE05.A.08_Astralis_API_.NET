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

        [Column("php_id")]
        [Required(ErrorMessage = "The phone prefix ID is required.")]
        public int PhonePrefixId { get; set; }

        [Column("cou_name")]
        [Required(ErrorMessage = "The name is required.")]
        [StringLength(80, ErrorMessage = "The name cannot be longer than 80 characters.")]
        public string Name { get; set; } = null!;

        [ForeignKey(nameof(PhonePrefixId))]
        [InverseProperty(nameof(PhonePrefix.Countries))]
        public virtual PhonePrefix PhonePrefixNavigation { get; set; } = null!;

        [InverseProperty(nameof(City.CountryNavigation))]
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
    }
}
