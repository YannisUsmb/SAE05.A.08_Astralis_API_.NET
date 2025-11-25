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
    
    // Users
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<CreditCard> CreditCards { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    // Addresses
    public virtual DbSet<Address> Addresses { get; set; }
    public virtual DbSet<City> Cities { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<PhonePrefix> PhonePrefixes { get; set; }

    // Articles & Comments
    public virtual DbSet<Article> Articles { get; set; }
    public virtual DbSet<ArticleInterest> ArticleInterests { get; set; }
    public virtual DbSet<ArticleType> ArticleTypes { get; set; }
    public virtual DbSet<TypeOfArticle> TypesOfArticle { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Report> Reports { get; set; }
    public virtual DbSet<ReportMotive> ReportMotives { get; set; }
    public virtual DbSet<ReportStatus> ReportStatuses { get; set; }

    // Celestial Bodies
    public virtual DbSet<CelestialBody> CelestialBodies { get; set; }
    public virtual DbSet<CelestialBodyType> CelestialBodyTypes { get; set; }
    public virtual DbSet<Planet> Planets { get; set; }
    public virtual DbSet<PlanetType> PlanetTypes { get; set; }
    public virtual DbSet<Satellite> Satellites { get; set; }
    public virtual DbSet<Star> Stars { get; set; }
    public virtual DbSet<SpectralClass> SpectralClasses { get; set; }
    public virtual DbSet<Asteroid> Asteroids { get; set; }
    public virtual DbSet<OrbitalClass> OrbitalClasses { get; set; }
    public virtual DbSet<Comet> Comets { get; set; }
    public virtual DbSet<GalaxyQuasar> GalaxiesQuasars { get; set; }
    public virtual DbSet<GalaxyQuasarClass> GalaxyQuasarClasses { get; set; }
    public virtual DbSet<DetectionMethod> DetectionMethods { get; set; }
    public virtual DbSet<Audio> Audios { get; set; }

    // Discoveries
    public virtual DbSet<Discovery> Discoveries { get; set; }
    public virtual DbSet<DiscoveryStatus> DiscoveryStatuses { get; set; }
    public virtual DbSet<AliasStatus> AliasStatuses { get; set; }

    // Events
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<EventInterest> EventInterests { get; set; }
    public virtual DbSet<EventType> EventTypes { get; set; }

    // Products
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductCategory> ProductCategories { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Command> Commands { get; set; }
    public virtual DbSet<CommandStatus> CommandStatuses { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Server=localhost;port=5432;Database=AstralisDB;uid=postgres;password=postgres;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.HasDefaultSchema("public");

        // ------- Users -------
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(uro => uro.Id).HasName("userrole_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id).HasName("user_pkey");

            entity.HasOne(u => u.UserRoleNavigation)
                .WithMany(ur => ur.Users)
                .HasForeignKey(u => u.UserRoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_userrole");

            entity.HasOne(u => u.PhonePrefixNavigation)
                .WithMany(pp => pp.Users)
                .HasForeignKey(u => u.PhonePrefixId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_user_phoneprefix");

            entity.HasOne(u => u.DeliveryAddressNavigation)
                .WithMany(a => a.DeliveryAddressUsers)
                .HasForeignKey(u => u.DeliveryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_user_deliveryaddress");

            entity.HasOne(u => u.InvoicingAddressNavigation)
                .WithMany(a => a.InvoicingAddressUsers)
                .HasForeignKey(u => u.InvoicingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_user_invoicingaddress");
        });

        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasKey(crc => crc.Id).HasName("creditcard_pkey");

            entity.HasOne(crc => crc.UserNavigation)
                .WithMany(u => u.CreditCards)
                .HasForeignKey(crc => crc.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_creditcard_user");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id).HasName("notification_pkey");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(un => new { un.UserId, un.NotificationId }).HasName("usernotification_pkey");

            entity.HasOne(un => un.UserNavigation)
                .WithMany(u => u.UserNotifications)
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_usernotification_user");

            entity.HasOne(un => un.NotificationNavigation)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_usernotification_notification");
        });


        // ------- Addresses -------
        modelBuilder.Entity<PhonePrefix>(entity =>
        {
            entity.HasKey(php => php.Id).HasName("phoneprefix_pkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(cou => cou.Id).HasName("country_pkey");

            entity.HasOne(cou => cou.PhonePrefixNavigation)
                .WithMany(php => php.Countries)
                .HasForeignKey(cou => cou.PhonePrefixId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_country_phoneprefix");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(cit => cit.Id).HasName("city_pkey");

            entity.HasOne(cit => cit.CountryNavigation)
                .WithMany(cou => cou.Cities)
                .HasForeignKey(cit => cit.CountryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_city_country");
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(add => add.Id).HasName("address_pkey");

            entity.HasOne(add => add.CityNavigation)
                .WithMany(cit => cit.Addresses)
                .HasForeignKey(add => add.CityId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_address_city");
        });


        // ------- Articles & Comments -------
        modelBuilder.Entity<ArticleType>(entity =>
        {
            entity.HasKey(at => at.Id).HasName("articletype_pkey");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id).HasName("article_pkey");

            entity.HasOne(a => a.UserNavigation)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_article_user");
        });

        modelBuilder.Entity<TypeOfArticle>(entity =>
        {
            entity.HasKey(toa => new { toa.ArticleTypeId, toa.ArticleId }).HasName("typeofarticle_pkey");

            entity.HasOne(toa => toa.ArticleNavigation)
                .WithMany(a => a.TypesOfArticle)
                .HasForeignKey(toa => toa.ArticleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_typeofarticle_article");

            entity.HasOne(toa => toa.ArticleTypeNavigation)
                .WithMany(at => at.TypesOfArticle)
                .HasForeignKey(toa => toa.ArticleTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_typeofarticle_articletype");
        });

        modelBuilder.Entity<ArticleInterest>(entity =>
        {
            entity.HasKey(ai => new { ai.ArticleId, ai.UserId }).HasName("articleinterest_pkey");

            entity.HasOne(ai => ai.ArticleNavigation)
                .WithMany(a => a.ArticleInterests)
                .HasForeignKey(ai => ai.ArticleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_articleinterest_article");

            entity.HasOne(ai => ai.UserNavigation)
                .WithMany(u => u.ArticleInterests)
                .HasForeignKey(ai => ai.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_articleinterest_user");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(com => com.Id).HasName("comment_pkey");

            entity.HasOne(com => com.UserNavigation)
                .WithMany(u => u.Comments)
                .HasForeignKey(com => com.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_comment_user");

            entity.HasOne(com => com.ArticleNavigation)
                .WithMany(art => art.Comments)
                .HasForeignKey(com => com.ArticleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_comment_article");

            entity.HasOne(com => com.RepliesToNavigation)
                .WithMany(c => c.Comments)
                .HasForeignKey(com => com.RepliesToId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_comment_repliesto");
        });

        modelBuilder.Entity<ReportMotive>(entity =>
        {
            entity.HasKey(rem => rem.Id).HasName("reportmotive_pkey");
        });

        modelBuilder.Entity<ReportStatus>(entity =>
        {
            entity.HasKey(rst => rst.Id).HasName("reportstatus_pkey");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(rep => rep.Id).HasName("report_pkey");

            entity.HasOne(rep => rep.ReportMotiveNavigation)
                .WithMany(rem => rem.Reports)
                .HasForeignKey(rep => rep.ReportMotiveId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_reportmotive");

            entity.HasOne(rep => rep.ReportStatusNavigation)
                .WithMany(rst => rst.Reports)
                .HasForeignKey(rep => rep.ReportStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_reportstatus");

            entity.HasOne(rep => rep.CommentNavigation)
                .WithMany(com => com.Reports)
                .HasForeignKey(rep => rep.CommentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_report_comment");

            entity.HasOne(rep => rep.UserNavigation)
                .WithMany(u => u.Reports)
                .HasForeignKey(rep => rep.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_report_user");

            entity.HasOne(rep => rep.AdminNavigation)
                .WithMany(u => u.TreatedReports)
                .HasForeignKey(rep => rep.AdminId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_report_adminuser");
        });


        // ------- Celestial Bodies -------
        modelBuilder.Entity<CelestialBodyType>(entity =>
        {
            entity.HasKey(cbt => cbt.Id).HasName("celestialbodytype_pkey");
        });

        modelBuilder.Entity<CelestialBody>(entity =>
        {
            entity.HasKey(cb => cb.Id).HasName("celestialbody_pkey");

            entity.HasOne(cb => cb.CelestialBodyTypeNavigation)
                .WithMany(cbt => cbt.CelestialBodies)
                .HasForeignKey(cb => cb.CelestialBodyTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_celestialbody_celestialbodytype");
        });

        modelBuilder.Entity<Audio>(entity =>
        {
            entity.HasKey(a => a.Id).HasName("audio_pkey");

            entity.HasOne(a => a.CelestialBodyTypeNavigation)
                .WithMany(cbt => cbt.Audios)
                .HasForeignKey(a => a.CelestialBodyTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_audio_celestialbodytype");
        });

        modelBuilder.Entity<PlanetType>(entity =>
        {
            entity.HasKey(pt => pt.Id).HasName("planettype_pkey");
        });

        modelBuilder.Entity<DetectionMethod>(entity =>
        {
            entity.HasKey(dm => dm.Id).HasName("detectionmethod_pkey");
        });

        modelBuilder.Entity<Planet>(entity =>
        {
            entity.HasKey(p => p.Id).HasName("planet_pkey");

            entity.HasOne(p => p.CelestialBodyNavigation)
                .WithOne(cb => cb.PlanetNavigation)
                .HasForeignKey<Planet>(p => p.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_planet_celestialbody");

            entity.HasOne(p => p.PlanetTypeNavigation)
                .WithMany(pt => pt.Planets)
                .HasForeignKey(p => p.PlanetTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_planet_planettype");

            entity.HasOne(p => p.DetectionMethodNavigation)
                .WithMany(dm => dm.Planets)
                .HasForeignKey(p => p.DetectionMethodId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_planet_detectionmethod");
        });

        modelBuilder.Entity<Satellite>(entity =>
        {
            entity.HasKey(s => s.Id).HasName("satellite_pkey");

            entity.HasOne(s => s.CelestialBodyNavigation)
                .WithOne(cb => cb.SatelliteNavigation)
                .HasForeignKey<Satellite>(s => s.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_satellite_celestialbody");

            entity.HasOne(s => s.PlanetNavigation)
                .WithMany(p => p.Satellites)
                .HasForeignKey(s => s.PlanetId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_satellite_planet");
        });

        modelBuilder.Entity<SpectralClass>(entity =>
        {
            entity.HasKey(spc => spc.Id).HasName("spectralclass_pkey");
        });

        modelBuilder.Entity<Star>(entity =>
        {
            entity.HasKey(s => s.Id).HasName("star_pkey");

            entity.HasOne(s => s.CelestialBodyNavigation)
                .WithOne(cb => cb.StarNavigation)
                .HasForeignKey<Star>(s => s.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_star_celestialbody");

            entity.HasOne(s => s.SpectralClassNavigation)
                .WithMany(spc => spc.Stars)
                .HasForeignKey(s => s.SpectralClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_star_spectralclass");
        });

        modelBuilder.Entity<OrbitalClass>(entity =>
        {
            entity.HasKey(oc => oc.Id).HasName("orbitalclass_pkey");
        });

        modelBuilder.Entity<Asteroid>(entity =>
        {
            entity.HasKey(a => a.Id).HasName("asteroid_pkey");

            entity.HasOne(a => a.CelestialBodyNavigation)
                .WithOne(cb => cb.AsteroidNavigation)
                .HasForeignKey<Asteroid>(a => a.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_asteroid_celestialbody");

            entity.HasOne(a => a.OrbitalClassNavigation)
                .WithMany(oc => oc.Asteroids)
                .HasForeignKey(a => a.OrbitalClassId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_asteroid_orbitalclass");
        });

        modelBuilder.Entity<Comet>(entity =>
        {
            entity.HasKey(c => c.Id).HasName("comet_pkey");

            entity.HasOne(c => c.CelestialBodyNavigation)
                .WithOne(cb => cb.CometNavigation)
                .HasForeignKey<Comet>(c => c.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_comet_celestialbody");
        });

        modelBuilder.Entity<GalaxyQuasarClass>(entity =>
        {
            entity.HasKey(gqc => gqc.Id).HasName("galaxyquasarclass_pkey");
        });

        modelBuilder.Entity<GalaxyQuasar>(entity =>
        {
            entity.HasKey(gq => gq.Id).HasName("galaxyquasar_pkey");

            entity.HasOne(gq => gq.CelestialBodyNavigation)
                .WithOne(cb => cb.GalaxyQuasarNavigation)
                .HasForeignKey<GalaxyQuasar>(gq => gq.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_galaxyquasar_celestialbody");

            entity.HasOne(gq => gq.GalaxyQuasarClassNavigation)
                .WithMany(gqc => gqc.GalaxiesQuasars)
                .HasForeignKey(gq => gq.GalaxyQuasarClassId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_galaxyquasar_galaxyquasarclass");
        });

        // ------- Discoveries -------
        modelBuilder.Entity<DiscoveryStatus>(entity =>
        {
            entity.HasKey(dst => dst.Id).HasName("discoverystatus_pkey");
        });

        modelBuilder.Entity<AliasStatus>(entity =>
        {
            entity.HasKey(als => als.Id).HasName("aliasstatus_pkey");
        });

        modelBuilder.Entity<Discovery>(entity =>
        {
            entity.HasKey(d => d.Id).HasName("discovery_pkey");

            entity.HasOne(d => d.CelestialBodyNavigation)
                .WithOne(cb => cb.DiscoveryNavigation)
                .HasForeignKey<Discovery>(d => d.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_discovery_celestialbody");

            entity.HasOne(d => d.DiscoveryStatusNavigation)
                .WithMany(dst => dst.Discoveries)
                .HasForeignKey(d => d.DiscoveryStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_discovery_discoverystatus");

            entity.HasOne(d => d.AliasStatusNavigation)
                .WithMany(als => als.Discoveries)
                .HasForeignKey(d => d.AliasStatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_discovery_aliasstatus");

            entity.HasOne(d => d.UserNavigation)
                .WithMany(u => u.Discoveries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_discovery_user");

            entity.HasOne(d => d.ApprovalUserNavigation)
                .WithMany(u => u.ApprovedDiscoveries)
                .HasForeignKey(d => d.DiscoveryApprovalUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_discovery_approvaluser");

            entity.HasOne(d => d.ApprovalAliasUserNavigation)
                .WithMany(u => u.ApprovedAliasDiscoveries)
                .HasForeignKey(d => d.AliasApprovalUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_discovery_aliasapprovaluser");
        });

        // ------- Events -------
        modelBuilder.Entity<EventType>(entity =>
        {
            entity.HasKey(evt => evt.Id).HasName("eventtype_pkey");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("event_pkey");

            entity.HasOne(e => e.EventTypeNavigation)
                .WithMany(evt => evt.Events)
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_event_eventtype");

            entity.HasOne(e => e.UserNavigation)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_event_user");
        });

        modelBuilder.Entity<EventInterest>(entity =>
        {
            entity.HasKey(ei => new { ei.EventId, ei.UserId }).HasName("eventinterest_pkey");

            entity.HasOne(ei => ei.EventNavigation)
                .WithMany(e => e.EventInterests)
                .HasForeignKey(ei => ei.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_eventinterest_event");

            entity.HasOne(ei => ei.UserNavigation)
                .WithMany(u => u.EventInterests)
                .HasForeignKey(ei => ei.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_eventinterest_user");
        });

        // ------- Products -------
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(prc => prc.Id).HasName("productcategory_pkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id).HasName("product_pkey");

            entity.HasOne(p => p.ProductCategoryNavigation)
                .WithMany(prc => prc.Products)
                .HasForeignKey(p => p.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_product_productcategory");

            entity.HasOne(p => p.UserNavigation)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_product_user");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => new { ci.UserId, ci.ProductId }).HasName("cartitem_pkey");

            entity.HasOne(ci => ci.UserNavigation)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_cartitem_user");

            entity.HasOne(ci => ci.ProductNavigation)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_cartitem_product");
        });

        modelBuilder.Entity<CommandStatus>(entity =>
        {
            entity.HasKey(cos => cos.Id).HasName("commandstatus_pkey");
        });

        modelBuilder.Entity<Command>(entity =>
        {
            entity.HasKey(c => c.Id).HasName("command_pkey");

            entity.HasOne(c => c.UserNavigation)
                .WithMany(u => u.Commands)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_command_user");

            entity.HasOne(c => c.CommandStatusNavigation)
                .WithMany(cos => cos.Commands)
                .HasForeignKey(c => c.CommandStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_command_commandstatus");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(od => new { od.CommandId, od.ProductId }).HasName("orderdetail_pkey");

            entity.HasOne(od => od.CommandNavigation)
                .WithMany(c => c.OrderDetails)
                .HasForeignKey(od => od.CommandId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_orderdetail_command");

            entity.HasOne(od => od.ProductNavigation)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_orderdetail_product");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}