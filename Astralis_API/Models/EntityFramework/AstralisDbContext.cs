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

    // Reports
    public virtual DbSet<Report> Reports { get; set; }
    public virtual DbSet<ReportMotive> ReportMotives { get; set; }
    public virtual DbSet<ReportStatus> ReportStatuses { get; set; }

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

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(a => a.Id).HasName("adress_pkey");

            entity.HasOne(a => a.CityNavigation)
                .WithMany(c => c.Addresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
