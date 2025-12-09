using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Astralis_API.Tests.Controllers
{
    public abstract class BaseControllerTests<TEntity, TController>
        where TEntity : class, new()
        where TController : class
    {
        protected AstralisDbContext _context;
        protected IMapper _mapper;
        protected TController _controller;

        protected abstract TController CreateController(AstralisDbContext context, IMapper mapper);
        protected abstract List<TEntity> GetSampleEntities();

        [TestInitialize]
        public virtual void BaseInitialize()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            var testConnectionString = configuration.GetConnectionString("DefaultConnection");
            var options = new DbContextOptionsBuilder<AstralisDbContext>()
                .UseNpgsql(testConnectionString)
                .Options;

            _context = new AstralisDbContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Models.Mapper.MapperProfile());
            });
            _mapper = config.CreateMapper();
            var samples = GetSampleEntities();
            _context.Set<TEntity>().AddRange(samples);
            _context.SaveChanges();
            _controller = CreateController(_context, _mapper);
        }
    }
}