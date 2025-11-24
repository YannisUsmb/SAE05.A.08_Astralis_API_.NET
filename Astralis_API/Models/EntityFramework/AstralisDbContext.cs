using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Models.EntityFramework;

public partial class AstralisDbContext : DbContext
{
    public AstralisDbContext()
    {
    }

    public AstralisDbContext(DbContextOptions<AstralisDbContext> options)
        : base(options)
    {
    }

    // Adresses
    public virtual DbSet<Address> Addresses { get; set; }
    public virtual DbSet<City> Cities { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<PhonePrefix> PhonePrefixes { get; set; }

    // Articles
    public virtual DbSet<Article> Articles { get; set; }
    public virtual DbSet<ArticleInterest> ArticleInterests { get; set; }
    public virtual DbSet<ArticleType> ArticleTypes { get; set; }
    public virtual DbSet<TypeOfArticle> TypesOfArticle { get; set; }

    // Celestial Bodies 
    public virtual DbSet<OrbitalClass> OrbitalClasses { get; set; }
    public virtual DbSet<Asteroid> Asteroids { get; set; }
    public virtual DbSet<Audio> Audios { get; set; }
    public virtual DbSet<CelestialBody> CelestialBodies { get; set; }
    public virtual DbSet<CelestialBodyType> CelestialBodyTypes { get; set; }
    public virtual DbSet<Comet> Comets { get; set; }
    public virtual DbSet<AliasStatus> AliasStatuses { get; set; }
    public virtual DbSet<Discovery> Discoveries { get; set; }
    public virtual DbSet<DiscoveryStatus> DiscoveryStatuses { get; set; }
    public virtual DbSet<GalaxyQuasar> GalaxiesQuasars { get; set; }
    public virtual DbSet<GalaxyQuasarClass> GalaxyQuasarClasses { get; set; }
    public virtual DbSet<DetectionMethod> DetectionMethods { get; set; }
    public virtual DbSet<Planet> Planets { get; set; }
    public virtual DbSet<PlanetType> PlanetTypes { get; set; }
    public virtual DbSet<Satellite> Satellites { get; set; }
    public virtual DbSet<SpectralClass> SpectralClasses { get; set; }
    public virtual DbSet<Star> Stars { get; set; }

    // Comments
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Report> Reports { get; set; }
    public virtual DbSet<ReportMotive> ReportMotives { get; set; }
    public virtual DbSet<ReportStatus> ReportStatuses { get; set; }

    // Events
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<EventInterest> EventInterests { get; set; }
    public virtual DbSet<EventType> EventTypes { get; set; }

    // Products
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Command> Commands { get; set; }
    public virtual DbSet<CommandStatus> CommandStatuses { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    // Users
    public virtual DbSet<CreditCard> CreditCards { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserNotification> UserNotifications { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=localhost;port=5432;Database=AstralisDB; uid=postgres;password=postgres;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.HasDefaultSchema("public");

        // Addresses
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(a => a.Id).HasName("address_pkey");

            entity.HasOne(a => a.CityNavigation)
                .WithMany(c => c.Addresses)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Address_City");

            entity.HasMany(a => a.InvoicingAddressUsers)
                .WithOne(u => u.InvoicingAddressNavigation)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_User_InvoicingAddress");

            entity.HasMany(a => a.DeliveryAddressUsers)
                .WithOne(u => u.DeliveryAddressNavigation)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_User_DeliveryAddress");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id).HasName("city_pkey");

            entity.HasOne(c => c.CountryNavigation)
                .WithMany(c => c.Cities)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_City_Country");

            entity.HasMany(c => c.Addresses)
                .WithOne(a => a.CityNavigation)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Address_City");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(c => c.Id).HasName("country_pkey");

            entity.HasOne(c => c.PhonePrefixNavigation)
                .WithMany(p => p.Countries)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Country_PhonePrefix");

            entity.HasMany(c => c.Cities)
                .WithOne(c => c.CountryNavigation)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_City_Country");
        });

        modelBuilder.Entity<PhonePrefix>(entity =>
        {
            entity.HasKey(p => p.Id).HasName("phoneprefix_pkey");

            entity.HasMany(p => p.Countries)
                .WithOne(c => c.PhonePrefixNavigation)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Country_PhonePrefix");

            entity.HasMany(p => p.Users)
                .WithOne(u => u.PhonePrefixNavigation)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_User_PhonePrefix");
        });

        // Articles
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id).HasName("article_pkey");

            entity.HasOne(a => a.UserNavigation)
                .WithMany(u => u.Articles)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Article_User");

            entity.HasMany(a => a.Comments)
                .WithOne(c => c.ArticleNavigation)
                .OnDelete(DeleteBehavior.Restrict); // Le nom de contrainte sera géré par la table Comment
        });

        modelBuilder.Entity<ArticleType>(entity =>
        {
            entity.HasKey(at => at.Id).HasName("articletype_pkey");
        });

        modelBuilder.Entity<TypeOfArticle>(entity =>
        {
            entity.HasOne(toa => toa.ArticleNavigation)
                .WithMany(a => a.TypesOfArticle)
                .OnDelete(DeleteBehavior.Cascade) // Si un Article est supprimé, la jointure doit l'être aussi.
                .HasConstraintName("FK_TypeOfArticle_Article");

            entity.HasOne(toa => toa.ArticleTypeNavigation)
                .WithMany(at => at.TypesOfArticle)
                .OnDelete(DeleteBehavior.Cascade) // Si un ArticleType est supprimé, la jointure doit l'être aussi.
                .HasConstraintName("FK_TypeOfArticle_ArticleType");
        });

        modelBuilder.Entity<ArticleInterest>(entity =>
        {
            entity.HasOne(ai => ai.ArticleNavigation)
                .WithMany(a => a.ArticleInterests)
                .OnDelete(DeleteBehavior.Cascade) // Si l'Article est supprimé, l'intérêt disparaît.
                .HasConstraintName("FK_ArticleInterest_Article");

            entity.HasOne(ai => ai.UserNavigation)
                .WithMany(u => u.ArticleInterests)
                .OnDelete(DeleteBehavior.Cascade) // Si l'User est supprimé, ses intérêts disparaissent.
                .HasConstraintName("FK_ArticleInterest_User");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
