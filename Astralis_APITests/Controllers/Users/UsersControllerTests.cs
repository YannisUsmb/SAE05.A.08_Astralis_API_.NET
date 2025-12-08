using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // Nécessaire pour IDbContextTransaction
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Astralis_API.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests : CrudControllerTests<User, UsersController, UserDetailDto, UserDetailDto, UserCreateDto, UserUpdateDto, int>
    {
        private IDbContextTransaction _transaction; // Variable pour stocker la transaction

        // Wrapper concret pour le Repository
        private class TestUserRepository : IUserRepository
        {
            private readonly AstralisDbContext _context;
            public TestUserRepository(AstralisDbContext context) { _context = context; }

            public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();
            public async Task<User?> GetByIdAsync(int id) => await _context.Users.FindAsync(id);
            public async Task AddAsync(User entity) { _context.Users.Add(entity); await _context.SaveChangesAsync(); }

            public async Task UpdateAsync(User entityToUpdate, User entity)
            {
                _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(User entity) { _context.Users.Remove(entity); await _context.SaveChangesAsync(); }
            public Task<User?> GetUserByEmailAsync(string email) => _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        [TestInitialize]
        public override void BaseInitialize()
        {
            // 1. Connexion à la BDD existante
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<AstralisDbContext>()
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            _context = new AstralisDbContext(builder.Options);

            _transaction = _context.Database.BeginTransaction();

            // 3. Config Mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Astralis_API.Models.Mapper.MapperProfile());
                cfg.CreateMap<User, UserCreateDto>();
                cfg.CreateMap<User, UserUpdateDto>();
                cfg.CreateMap<ChangePasswordDto, User>()
                   .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.NewPassword));
            });

            _mapper = config.CreateMapper();

            var samples = GetSampleEntities();

            // On vérifie si les IDs existent déjà pour éviter de planter
            foreach (var user in samples)
            {
                var existingUser = _context.Users.Find(user.Id);
                if (existingUser == null)
                {
                    // Pour éviter les conflits FK, on s'assure que le UserRole est attaché mais pas recréé
                    if (user.UserRoleNavigation != null)
                    {
                        // On attache le rôle existant (2 ou 4) au contexte pour ne pas essayer de le réinsérer
                        _context.Attach(user.UserRoleNavigation);
                    }
                    _context.Users.Add(user);
                }
            }

            // On sauvegarde (cela reste dans la transaction, donc temporaire)
            _context.SaveChanges();

            _controller = CreateController(_context, _mapper);
        }

        [TestCleanup]
        public override void BaseCleanup()
        {
            // ⚠️ ANNULATION DES MODIFICATIONS
            // Ceci remet la base de données dans l'état exact où elle était avant le test.
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }

            _context.Dispose();
        }

        protected override UsersController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var repo = new TestUserRepository(context);
            return new UsersController(repo, mapper);
        }

        protected override int GetEntityId(User entity) => entity.Id;

        protected override List<User> GetSampleEntities()
        {
            // On utilise les IDs existants de vos rôles (2 et 4)
            // On ne crée pas de "new UserRole" ici pour éviter les erreurs de tracking EF
            var roleClient = new UserRole { Id = 2, Label = "Client" };
            var roleAdmin = new UserRole { Id = 4, Label = "Admin" };

            return new List<User>
            {
                // J'utilise des IDs négatifs ou élevés pour éviter de tomber sur vos vrais users
                // Ou alors assurez-vous que ces IDs sont libres dans votre base.
                new User
                {
                    Id = 1001, // ID arbitraire pour le test
                    Username = "TestUser_Client",
                    FirstName = "Test",
                    LastName = "Client",
                    Email = "test_client@test.com",
                    Password = "Password123!",
                    UserRoleNavigation = roleClient,
                    IsPremium = false
                },
                new User
                {
                    Id = 1002,
                    Username = "TestUser_Admin",
                    FirstName = "Test",
                    LastName = "Admin",
                    Email = "test_admin@test.com",
                    Password = "Password123!",
                    UserRoleNavigation = roleAdmin,
                    IsPremium = true
                }
            };
        }

        protected override void UpdateEntityForTest(User entity)
        {
            entity.Username = "UpdatedUserTest";
            entity.Email = "updated_test@test.com";
            entity.Password = "Password123!";
            entity.FirstName = "UpdatedFirst";
            entity.LastName = "UpdatedLast";

            // On récupère un rôle existant (Client = 2) sans faire de requête BDD si possible
            // En créant un objet stub avec juste l'ID
            entity.UserRoleNavigation = new UserRole { Id = 2 };
            _context.Attach(entity.UserRoleNavigation); // On l'attache pour dire à EF qu'il existe déjà
        }

        // --- Helper Sécurité ---
        private void SetupHttpContext(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, "TestUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        // --- Surcharges Actions ---

        protected override async Task<ActionResult<IEnumerable<UserDetailDto>>> ActionGetAll()
        {
            SetupHttpContext(999, "Admin");
            return await _controller.GetAll();
        }

        protected override async Task<ActionResult<UserDetailDto>> ActionGetById(int id)
        {
            SetupHttpContext(id, "Client");
            return await _controller.GetById(id);
        }

        protected override async Task<ActionResult<UserDetailDto>> ActionPost(UserCreateDto dto)
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            return await _controller.Post(dto);
        }

        protected override async Task<IActionResult> ActionPut(int id, UserUpdateDto dto)
        {
            SetupHttpContext(id, "Client");
            return await _controller.Put(id, dto);
        }

        protected override async Task<IActionResult> ActionDelete(int id)
        {
            SetupHttpContext(999, "Admin");
            return await _controller.Delete(id);
        }

        // --- Tests Spécifiques ---

        [TestMethod]
        public async Task ChangePassword_ShouldUpdatePassword_WhenUserIsSelf()
        {
            // On prend un user créé par le test (ID 1001)
            var userId = 1001;
            // On s'assure qu'il est chargé (normalement fait dans Initialize)
            var user = await _context.Users.FindAsync(userId);

            // Si jamais GetSampleEntities a échoué car l'ID existait déjà, on prend le premier dispo
            if (user == null) user = await _context.Users.FirstAsync();

            var newPasswordDto = new ChangePasswordDto { NewPassword = "NewPassword123!" };

            SetupHttpContext(user.Id, "Client");

            var result = await _controller.ChangePassword(user.Id, newPasswordDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.Entry(user).Reload();
            Assert.AreEqual("NewPassword123!", user.Password);
        }

        [TestMethod]
        public async Task ChangePassword_ShouldReturnForbid_WhenUserIsNotSelf()
        {
            var userId = 1001;
            var user = await _context.Users.FindAsync(userId);
            if (user == null) user = await _context.Users.FirstAsync();

            var newPasswordDto = new ChangePasswordDto { NewPassword = "NewPassword123!" };

            SetupHttpContext(user.Id + 1, "Client");

            var result = await _controller.ChangePassword(user.Id, newPasswordDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}