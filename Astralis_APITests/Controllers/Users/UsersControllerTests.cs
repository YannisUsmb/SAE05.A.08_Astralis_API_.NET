using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Astralis_API.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests : CrudControllerTests<User, UsersController, UserDetailDto, UserDetailDto, UserCreateDto, UserUpdateDto, int>
    {
        private IDbContextTransaction _transaction;

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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<AstralisDbContext>()
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            _context = new AstralisDbContext(builder.Options);

            _transaction = _context.Database.BeginTransaction();            
            var samples = GetSampleEntities();

            foreach (var user in samples)
            {
                var existingUser = _context.Users.Find(user.Id);
                if (existingUser == null)
                {
                    if (user.UserRoleNavigation != null)
                    {
                        _context.Attach(user.UserRoleNavigation);
                    }
                    _context.Users.Add(user);
                }
            }

            _context.SaveChanges();

            _controller = CreateController(_context, _mapper);
        }

        [TestCleanup]
        public void BaseCleanup()
        {
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
            return new List<User>
            {
                new User
                {
                    Id = 1001,
                    Username = "TestUser_Client",
                    FirstName = "Test",
                    LastName = "Client",
                    Email = "test_client@test.com",
                    Password = "Password123!",
                    UserRoleId= 2,
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
                    UserRoleId= 4,
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

            entity.UserRoleNavigation = new UserRole { Id = 2 };
            _context.Attach(entity.UserRoleNavigation);
        }

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


        [TestMethod]
        public async Task ChangePassword_ShouldUpdatePassword_WhenUserIsSelf()
        {
            var userId = 1001;
            var user = await _context.Users.FindAsync(userId);

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