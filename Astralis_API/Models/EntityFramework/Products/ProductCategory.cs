using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_productcategory_prc")]
    public class ProductCategory
    {
        [Key]
        [Column("prc_id")]
        public int Id { get; set; }

        [Column("prc_label")]
        [Required(ErrorMessage = "The product category label is required.")]
        [StringLength(100, ErrorMessage = "The product category label length must not be over 100 characters.")]
        public string Label { get; set; } = null!;

        [Column("prc_description")]
        [StringLength(300, ErrorMessage = "The product category description length must not be over 300 characters.")]
        public string? Description { get; set; }

        [InverseProperty(nameof(Product.ProductCategoryNavigation))]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
