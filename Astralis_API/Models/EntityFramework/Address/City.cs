using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_city_cit")]
    public class City
    {
        [Key]
        [Column("cit_id")]
        public int Id { get; set; }

        [Column("cou_id")]
        [Required(ErrorMessage = "The country ID is required.")]
        public int CountryId { get; set; }

        [Column("cit_name")]
        [Required(ErrorMessage = "The city name is required.")]
        [StringLength(100, ErrorMessage = "The city name cannot be longer than 100 characters.")]
        public string Name { get; set; } = null!;

        [Column("cit_postcode")]
        [Required(ErrorMessage = "The city postcode is required.")]
        [StringLength(20, ErrorMessage = "The city postcode cannot be longer than 20 characters.")]
        public string PostCode { get; set; } = null!;
        
        [Column("cit_latitude", TypeName ="NUMERIC(10,7)")]
        public double Latitude { get; set; }

        [Column("cit_longitude", TypeName ="NUMERIC(10,7)")]
        public double Longitude { get; set; }

        [ForeignKey(nameof(CountryId))]
        [InverseProperty(nameof(Country.Cities))]
        public virtual Country Country { get; set; } = null!;

        [InverseProperty(nameof(Address.City))]
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    }
}
