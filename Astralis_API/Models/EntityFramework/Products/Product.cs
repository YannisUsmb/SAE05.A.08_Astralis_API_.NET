using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_product_pro")]
    public class Product
    {
        [Key]
        [Column("pro_id")]
        public int Id { get; set; }

        [Column("prc_id")]
        [Required(ErrorMessage = "The product category ID is required.")]
        public int ProductCategoryId { get; set; }

        [Column("usr_id")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

        [Column("pro_label")]
        [Required(ErrorMessage = "The label is required.")]
        [StringLength(100, ErrorMessage = "The label cannot be longer than 100 characters.")]
        public string Label { get; set; } = null!;

        [Column("pro_description")]
        [StringLength(300, ErrorMessage = "The description cannot be longer than 300 characters.")]
        public string? Description { get; set; }

        [Column("pro_price")]
        [Required(ErrorMessage = "The price is required.")]
        [Range(0.01, 9999.99, ErrorMessage = "Price must be positive.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [ForeignKey(nameof(ProductCategoryId))]
        [InverseProperty(nameof(ProductCategory.Products))]
        public virtual ProductCategory ProductCategory { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Products))]
        public virtual User User { get; set; } = null!;

        [InverseProperty(nameof(Cart.Product))]
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        [InverseProperty(nameof(OrderDetail.Product))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
