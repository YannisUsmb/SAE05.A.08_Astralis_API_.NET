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

        [Column("add_streetnumber")]
        [Required(ErrorMessage = "The street number is required.")]
        [StringLength(15, ErrorMessage = "The street number length must not be over 15 characters.")]
        public String StreetNumber { get; set; }

        [Column("add_streetaddress")]
        [Required(ErrorMessage = "The street address is required.")]
        [StringLength(200, ErrorMessage = "The street address length must not be over 200 characters.")]
        public String StreetAddress { get; set; }

    }
}
