using Astralis_API.Configuration;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// -- Services -- //

// Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    doc =>
    {
        // Configure Swagger to use the XML documentation file.
        var xmlFile = Path.ChangeExtension(typeof(Program).Assembly.Location, ".xml");
        doc.IncludeXmlComments(xmlFile);
        doc.OperationFilter<SwaggerGenericFilter>();
    }
);

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AstralisDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Dependency Injection for Data Managers
//      Addresses
builder.Services.AddScoped<IAddressRepository, AddressManager>();
builder.Services.AddScoped<ICityRepository, CityManager>();
builder.Services.AddScoped<ICountryRepository, CountryManager>();
builder.Services.AddScoped<IPhonePrefixRepository, PhonePrefixManager>();

//      Articles
builder.Services.AddScoped<IArticleInterestRepository, ArticleInterestManager>();
builder.Services.AddScoped<IArticleRepository, ArticleManager>();
builder.Services.AddScoped<IArticleTypeRepository, ArticleTypeManager>();
builder.Services.AddScoped<ITypeOfArticleRepository, TypeOfArticleManager>();

//      Celestial bodies
//          Asteroid
builder.Services.AddScoped<IAsteroidRepository, AsteroidManager>();
builder.Services.AddScoped<IOrbitalClassRepository, OrbitalClassManager>();

//          Audio
builder.Services.AddScoped<IAudioRepository, AudioManager>();

//          Celestial body
builder.Services.AddScoped<ICelestialBodyRepository, CelestialBodyManager>();
builder.Services.AddScoped<ICelestialBodyTypeRepository, CelestialBodyTypeManager>();

//          Comet
builder.Services.AddScoped<ICometRepository, CometManager>();

//          Discovery
builder.Services.AddScoped<IAliasStatusRepository, AliasStatusManager>();
builder.Services.AddScoped<IDiscoveryRepository, DiscoveryManager>();
builder.Services.AddScoped<IDiscoveryStatusRepository, DiscoveryStatusManager>();

//          Galaxy Quasar
builder.Services.AddScoped<IGalaxyQuasarClassRepository, GalaxyQuasarClassManager>();
builder.Services.AddScoped<IGalaxyQuasarRepository, GalaxyQuasarManager>();

//          Planet
builder.Services.AddScoped<IDetectionMethodRepository, DetectionMethodManager>();
builder.Services.AddScoped<IPlanetRepository, PlanetManager>();
builder.Services.AddScoped<IPlanetTypeRepository, PlanetTypeManager>();

//          Satellite
builder.Services.AddScoped<ISatelliteRepository, SatelliteManager>();

//          Star
builder.Services.AddScoped<IStarRepository, StarManager>();
builder.Services.AddScoped<ISpectralClassRepository, SpectralClassManager>();

//      Comments
builder.Services.AddScoped<ICommentRepository, CommentManager>();
builder.Services.AddScoped<IReportRepository, ReportManager>();
builder.Services.AddScoped<IReportMotiveRepository, ReportMotiveManager>();
builder.Services.AddScoped<IReportStatusRepository, ReportStatusManager>();

//      Events
builder.Services.AddScoped<IEventInterestRepository, EventInterestManager>();
builder.Services.AddScoped<IEventRepository, EventManager>();
builder.Services.AddScoped<IEventTypeRepository, EventTypeManager>();

//      Products
builder.Services.AddScoped<ICartItemRepository, CartItemManager>();
builder.Services.AddScoped<ICommandRepository, CommandManager>();
builder.Services.AddScoped<ICommandStatusRepository, CommandStatusManager>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailManager>();
builder.Services.AddScoped<IProductRepository, ProductManager>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryManager>();

//      Users
builder.Services.AddScoped<ICreditCardRepository, CreditCardManager>();
builder.Services.AddScoped<INotificationRepository, NotificationManager>();
builder.Services.AddScoped<IUserRepository, UserManager>();
builder.Services.AddScoped<IUserNotificationRepository, UserNotificationManager>();
builder.Services.AddScoped<IUserRoleRepository,UserRoleManager>();

//      Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy
            .WithOrigins("https://localhost:7200") // à modifier
            .AllowAnyHeader()
            .AllowAnyMethod());
});


// -- Pipeline -- //

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazor");

app.UseAuthorization();

app.MapControllers();

app.Run();