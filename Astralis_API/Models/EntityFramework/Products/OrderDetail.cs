using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_j_orderdetail_ord")]
    public class OrderDetail
    {
        [Column("cmd_id")]
        [Required(ErrorMessage = "The command ID is required.")]
        public int CommandId { get; set; }

        [Column("pro_id")]
        [Required(ErrorMessage = "The product ID is required.")]
        public int ProductId { get; set; }

        [Column("ord_quantity")]
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [ForeignKey(nameof(CommandId))]
        [InverseProperty(nameof(Command.OrderDetails))]
        public virtual Command CommandNavigation { get; set; } = null!;

        [ForeignKey(nameof(ProductId))]
        [InverseProperty(nameof(Product.OrderDetails))]
        public virtual Product ProductNavigation { get; set; } = null!;
    }
}
