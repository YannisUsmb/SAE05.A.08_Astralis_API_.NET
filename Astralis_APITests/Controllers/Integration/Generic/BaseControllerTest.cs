using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Mapper;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace Astralis_APITests.Controllers
{
    public abstract class BaseControllerTests<TEntity, TController>
        where TEntity : class, new()
        where TController : class
    {
        protected AstralisDbContext _context;
        protected IMapper _mapper;
        protected TController _controller;
        protected IDbContextTransaction _transaction;

        protected List<TEntity> _seededEntities;

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

            _context.Database.OpenConnection();
            _transaction = _context.Database.BeginTransaction();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            _mapper = config.CreateMapper();

            _seededEntities = GetSampleEntities();
            if (_seededEntities != null && _seededEntities.Any())
            {
                _context.Set<TEntity>().AddRange(_seededEntities);
                _context.SaveChanges();
            }

            _controller = CreateController(_context, _mapper);
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
            _context.Dispose();
        }
    }
}