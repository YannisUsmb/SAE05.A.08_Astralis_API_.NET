using Astralis_API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Astralis_API.Models.EntityFramework
{
    [Table("t_e_user_usr")]
    public class User
    {
        [Key]
        [Column("usr_id")]
        public int Id { get; set; }

        [Column("php_id")]
        public int? PhonePrefixId { get; set; }

        [Column("add_iddelivery")]
        public int? DeliveryId { get; set; }

        [Column("add_idinvoicing")]
        public int? InvoicingId { get; set; }

        [Column("uro_id")]
        [Required(ErrorMessage = "The user role is required.")]
        public int UserRoleId { get; set; }

        [Column("usr_lastname")]
        [Required(ErrorMessage = "The user lastname is required.")]
        [StringLength(100, ErrorMessage = "The user lastname cannot be longer than 100 caracters.")]
        public string LastName { get; set; } = null!;

        [Column("usr_firstname")]
        [Required(ErrorMessage = "The user firstname is required.")]
        [StringLength(100, ErrorMessage = "The user firstname cannot be longer than 100 caracters.")]
        public string FirstName { get; set; } = null!;

        [Column("usr_email")]
        [Required(ErrorMessage = "The user email is required.")]
        [StringLength(250, ErrorMessage = "The user email cannot be longer than 250 caracters.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = null!;

        [Column("usr_phone")]
        [StringLength(20, ErrorMessage = "The user phone cannot be longer than 20 caracters.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; }

        [Column("usr_pseudo")]
        [Required(ErrorMessage = "The user pseudo is required.")]
        [StringLength(50, ErrorMessage = "The user pseudo cannot be longer than 50 caracters.")]
        public string Pseudo { get; set; } = null!;

        [Column("usr_password", TypeName = "CHAR(256)")]
        [Required(ErrorMessage = "The user password is required.")]
        public string Password { get; set; } = null!;

        [Column("usr_inscriptiondate")]
        [Required(ErrorMessage = "The user Inscription date is required.")]
        public DateTime InscriptionDate { get; set; } = DateTime.UtcNow;

        [Column("usr_gender")]
        public char? Gender { get; set; }
        
        [Required(ErrorMessage = "The user premium status is required.")]
        [Column("usr_ispremium")]
        public bool IsPremium { get; set; }

        [Required(ErrorMessage = "The user multi-factor authentication status is required.")]
        [Column("usr_multifactorauthentification")]
        public bool MultiFactorAuthentification { get; set; }

        [ForeignKey(nameof(PhonePrefixId))]
        [InverseProperty(nameof(PhonePrefix.Users))]
        public virtual PhonePrefix? PhonePrefixNavigation { get; set; }

        [ForeignKey(nameof(DeliveryId))]
        [InverseProperty(nameof(Address.DeliveryAddressUsers))]
        public virtual Address? DeliveryAddressNavigation { get; set; }

        [ForeignKey(nameof(InvoicingId))]
        [InverseProperty(nameof(Address.InvoicingAddressUsers))]
        public virtual Address? InvoicingAddressNavigation { get; set; }

        [ForeignKey(nameof(UserRoleId))]
        [InverseProperty(nameof(UserRole.Users))]
        public virtual UserRole? UserRoleNavigation { get; set; }


        [InverseProperty(nameof(CreditCard.UserNavigation))]
        public virtual ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();

        [InverseProperty(nameof(Event.UserNavigation))]
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();

        [InverseProperty(nameof(Command.UserNavigation))]
        public virtual ICollection<Command> Commands { get; set; } = new List<Command>();

        [InverseProperty(nameof(CartItem.UserNavigation))]
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        [InverseProperty(nameof(Product.UserNavigation))]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        [InverseProperty(nameof(EventInterest.UserNavigation))]
        public virtual ICollection<EventInterest> EventInterests { get; set; } = new List<EventInterest>();

        [InverseProperty(nameof(Article.UserNavigation))]
        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

        [InverseProperty(nameof(ArticleInterest.UserNavigation))]
        public virtual ICollection<ArticleInterest> ArticleInterests { get; set; } = new List<ArticleInterest>();

        [InverseProperty(nameof(Comment.UserNavigation))]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [InverseProperty(nameof(Report.UserNavigation))]
        public virtual ICollection<Report> Reports { get; set; } = new List<Reports>();

        [InverseProperty(nameof(Discovery.UserNavigation))]
        public virtual ICollection<Discovery> Discoveries { get; set; } = new List<Discovery>();

        [InverseProperty(nameof(Discovery.ApprovalUserNavigation))]
        public virtual ICollection<Discovery> ApprovedDiscoveries { get; set; } = new List<Discovery>();

        [InverseProperty(nameof(Discovery.ApprovalAliasUserNavigation))]
        public virtual ICollection<Discovery> ApprovedAliasDiscoveries { get; set; } = new List<Discovery>();

        [InverseProperty(nameof(UserNotification.UserNavigation))]
        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();

    }
}
