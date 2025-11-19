using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_address_add")]
    public class Address
    {
        [Key]
        [Column("add_id")]
        public int Id { get; set; }
                
        [Column("cit_id")]
        [Required(ErrorMessage = "The city ID is required.")]
        public int CityId { get; set; }

        [Column("add_streetnumber")]
        [Required(ErrorMessage = "The street number is required.")]
        [StringLength(15, ErrorMessage = "The street number cannot be longer than 15 characters.")]
        public string StreetNumber { get; set; } = null!;

        [Column("add_streetaddress")]
        [Required(ErrorMessage = "The street address is required.")]
        [StringLength(200, ErrorMessage = "The street address cannot be longer than 200 characters.")]
        public string StreetAddress { get; set; } = null!;

        [ForeignKey(nameof(CityId))]
        [InverseProperty(nameof(City.Addresses))]
        public virtual City CityNavigation { get; set; } = null!;


        [InverseProperty(nameof(User.InvoicingAddressNavigation))]
        public virtual ICollection<User> InvoicingAddressUsers { get; set; } = new List<User>();

        [InverseProperty(nameof(User.DeliveryAddressNavigation))]
        public virtual ICollection<User> DeliveryAddressUsers { get; set; } = new List<User>();
    }
}