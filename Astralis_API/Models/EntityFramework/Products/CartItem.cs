using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_cartitem_cai")]
    [PrimaryKey(nameof(UserId), nameof(ProductId))]
    public class CartItem
    {
        [Column("usr_id")]
        public int UserId { get; set; }

        [Column("pro_id")]
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
