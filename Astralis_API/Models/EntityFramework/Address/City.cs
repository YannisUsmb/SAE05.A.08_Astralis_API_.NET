using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e__")]
    public class City
    {
        [Key]
        [Column("_id")]
        public int Id { get; set; }

        [Column("_streetnumber")]
        [Required(ErrorMessage = "The is required.")]
        [StringLength(15, ErrorMessage = "The length must not be over 15 characters.")]
        public String StreetNumber { get; set; }

        [Column("_streetaddress")]
        [Required(ErrorMessage = "The  is required.")]
        [StringLength(200, ErrorMessage = "The  length must not be over 200 characters.")]
        public String StreetAddress { get; set; }

    }
}
