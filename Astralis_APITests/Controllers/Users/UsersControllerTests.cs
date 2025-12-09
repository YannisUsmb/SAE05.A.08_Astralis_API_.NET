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
        // --- 1. Configuration des Données ---

        protected override int GetEntityId(User entity) => entity.Id;
        protected override int GetNonExistentId()
        {
            return 0;
        }
        protected override List<User> GetSampleEntities()
            {
            // On prépare les users (IDs 1001+ pour éviter conflits)
            // Note : On ne met pas les Rôles ici, on les gère dans SeedDatabase
            return new List<User>
            {
                new User { Id = 1001, Username = "UserTest1", Email = "u1@t.com", Password = "Pwd", FirstName = "F1", LastName = "L1", IsPremium = false },
                new User { Id = 1002, Username = "UserTest2", Email = "u2@t.com", Password = "Pwd", FirstName = "F2", LastName = "L2", IsPremium = true }
            };
            }

        // Surcharge de SeedDatabase pour attacher proprement les Rôles existants (IDs 2 et 4)
        protected void SeedDatabase()
        {
            var roleClient = new UserRole { Id = 2 }; // ID Client existant
            var roleAdmin = new UserRole { Id = 4 };  // ID Admin existant

            // On attache pour ne pas créer de doublons
            _context.Attach(roleClient);
            _context.Attach(roleAdmin);

            var samples = GetSampleEntities();
            samples[0].UserRoleNavigation = roleClient;
            samples[1].UserRoleNavigation = roleAdmin;

            _context.Users.AddRange(samples);
            _context.SaveChanges();

            _controller = CreateController(_context, _mapper);
        }

        protected override void UpdateEntityForTest(User entity)
        {
            entity.Username = "UpdatedUser";
            entity.Email = "updated@test.com";
            entity.FirstName = "UpFirst";
            entity.LastName = "UpLast";
            entity.Password = "NewPass!";

            // Gestion FK pour l'update
            var role = new UserRole { Id = 2 };
            _context.Attach(role);
            entity.UserRoleNavigation = role;
        }

        // --- 2. Configuration du Contrôleur & Sécurité ---

        protected override UsersController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new UsersController(new TestUserRepository(context), mapper);
        }

        // Helper pour simuler l'authentification
        private void SetupHttpContext(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };
        }

        // --- 3. Liaison des Actions (Injection de la Sécurité) ---

        protected override Task<ActionResult<IEnumerable<UserDetailDto>>> ActionGetAll()
        {
            SetupHttpContext(999, "Admin");
            return _controller.GetAll();
        }

        protected override Task<ActionResult<UserDetailDto>> ActionGetById(int id)
        {
            SetupHttpContext(id, "Client"); // User qui consulte son propre profil
            return _controller.GetById(id);
        }

        protected override Task<ActionResult<UserDetailDto>> ActionPost(UserCreateDto dto)
        {
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            return _controller.Post(dto);
        }

        protected override Task<IActionResult> ActionPut(int id, UserUpdateDto dto)
        {
            SetupHttpContext(id, "Client");
            return _controller.Put(id, dto);
        }

        protected override Task<IActionResult> ActionDelete(int id)
        {
            SetupHttpContext(999, "Admin");
            return _controller.Delete(id);
        }


        [TestMethod]
        public async Task ChangePassword_ShouldUpdate_WhenSelf()
        {
            var user = await _context.Users.FirstAsync();
            SetupHttpContext(user.Id, "Client");
            var dto = new ChangePasswordDto { NewPassword = "NewPassword123!" }; // Assurez-vous que le DTO matche

            var result = await _controller.ChangePassword(user.Id, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.Entry(user).Reload();
            Assert.AreEqual("NewPassword123!", user.Password);
        }

        private class TestUserRepository : IUserRepository
        {
            private readonly AstralisDbContext _db;
            public TestUserRepository(AstralisDbContext db) { _db = db; }
            public async Task<IEnumerable<User>> GetAllAsync() => await _db.Users.ToListAsync();
            public async Task<User?> GetByIdAsync(int id) => await _db.Users.FindAsync(id);
            public async Task AddAsync(User e) { _db.Users.Add(e); await _db.SaveChangesAsync(); }
            public async Task UpdateAsync(User u, User e) { _db.Entry(u).CurrentValues.SetValues(e); await _db.SaveChangesAsync(); }
            public async Task DeleteAsync(User e) { _db.Users.Remove(e); await _db.SaveChangesAsync(); }
            public Task<User?> GetUserByEmailAsync(string email) => _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}