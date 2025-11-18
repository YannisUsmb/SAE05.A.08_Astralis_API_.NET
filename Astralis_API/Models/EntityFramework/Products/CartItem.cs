using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_cartitem_cai")]
    public class CartItem
    {
        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [Column("pro_id")]
        [Required(ErrorMessage = "The product ID is required.")]
        public int ProductId { get; set; }

        [Column("cai_quantity")]
        [Required(ErrorMessage = "The quantity is required.")]
        public int Quantity { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.CartItems))]
        public virtual User UserNavigation { get; set; } = null!;

        [ForeignKey(nameof(ProductId))]
        [InverseProperty(nameof(Product.CartItems))]
        public virtual Product ProductNavigation { get; set; } = null!;
    }
}
