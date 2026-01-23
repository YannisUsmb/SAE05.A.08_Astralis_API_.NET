using Astralis_API.Configuration;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Models.Repository.Specific;
using Astralis_API.Services.Implementations;
using Astralis_API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
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
builder.Services.AddSwaggerGen(c =>
{
    // Configure Swagger to use the XML documentation file.
    var xmlFile = Path.ChangeExtension(typeof(Program).Assembly.Location, ".xml");
    c.IncludeXmlComments(xmlFile);
    c.OperationFilter<SwaggerGenericFilter>();

    // Security definition for JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Security requirement for JWT Bearer
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AstralisDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Configuration JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
// Important : s'assurer que la clé est encodée pareil
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("authToken"))
            {
                context.Token = context.Request.Cookies["authToken"];
            }
            return Task.CompletedTask;
        }
    };
});

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
builder.Services.AddScoped<IJoinRepository<TypeOfArticle, int, int>, TypeOfArticleManager>();
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
builder.Services.AddScoped<INotificationTypeRepository, NotificationTypeManager>();
builder.Services.AddScoped<IUserRepository, UserManager>();
builder.Services.AddScoped<IUserNotificationRepository, UserNotificationManager>();
builder.Services.AddScoped<IUserNotificationTypeRepository, UserNotificationTypeManager>();
builder.Services.AddScoped<IUserRoleRepository,UserRoleManager>();

// Services
builder.Services.AddScoped<IEmailService, GmailEmailService>();
builder.Services.AddScoped<IUploadService, BlobStorageService>();
builder.Services.AddHttpClient<IAiService, AiService>();

// automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//      Cors
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',');

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
        }
    });
});

// -- Pipeline -- //

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseHttpsRedirection();

app.UseCors("AllowBlazor");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();