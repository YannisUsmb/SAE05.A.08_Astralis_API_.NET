using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq; // Important pour AsNoTracking
using System.Security.Claims;
using System.Threading.Tasks;

namespace Astralis_API.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests : CrudControllerTests<User, UsersController, UserDetailDto, UserDetailDto, UserCreateDto, UserUpdateDto, int>
    {
        // --- CONFIGURATION ---

        protected override int GetEntityId(User entity) => entity.Id;
        protected override int GetDtoId(UserDetailDto dto) => dto.Id;
        protected override int GetNonExistentId() => 0;

        protected override List<User> GetSampleEntities()
        {
            return new List<User>
            {
                new User { Id = 1001, Username = "TestUser1", Email = "t1@test.com", Password = "Pwd", UserRoleId=2, FirstName = "F1", LastName = "L1", IsPremium = false },
                new User { Id = 1002, Username = "TestUser2", Email = "t2@test.com", Password = "Pwd", UserRoleId=2, FirstName = "F2", LastName = "L2", IsPremium = true }
            };
        }

        // --- CORRECTION 1 : SeedDatabase Robuste ---
        protected void SeedDatabase()
        {
            // 1. On nettoie le tracker pour éviter les conflits avec ce qui s'est passé avant
            _context.ChangeTracker.Clear();

            // 2. On attache les rôles existants (2 et 4) pour éviter de les recréer
            var roleClient = new UserRole { Id = 2 };
            var roleAdmin = new UserRole { Id = 4 };
            _context.Attach(roleClient);
            _context.Attach(roleAdmin);

            var samples = GetSampleEntities();

            // On lie les rôles
            samples[0].UserRoleNavigation = roleClient;
            samples[1].UserRoleNavigation = roleAdmin;

            foreach (var user in samples)
            {
                // 3. Vérification Anti-Crash : On n'ajoute QUE si l'ID n'existe pas
                var exists = _context.Users.AsNoTracking().Any(u => u.Id == user.Id);
                if (!exists)
                {
                    _context.Users.Add(user);
                }
            }

            _context.SaveChanges();

            // 4. On détache tout pour que le Contrôleur parte sur des bases saines
            _context.ChangeTracker.Clear();
        }

        protected override void UpdateEntityForTest(User entity)
        {
            entity.Username = "UpdatedUser";
            entity.Email = "updated@test.com";
            entity.FirstName = "UpFirst";
            entity.LastName = "UpLast";
            entity.Password = "NewPass!";

            // Pour l'update, on réattache le rôle proprement
            var role = new UserRole { Id = 2 };
            // On vérifie si le rôle est déjà tracké avant de l'attacher
            var existingEntry = _context.ChangeTracker.Entries<UserRole>().FirstOrDefault(e => e.Entity.Id == 2);
            if (existingEntry == null)
            {
                _context.Attach(role);
                entity.UserRoleNavigation = role;
            }
            else
            {
                entity.UserRoleNavigation = existingEntry.Entity;
            }
        }

        protected override UsersController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new UsersController(new TestUserRepository(context), mapper);
        }

        // --- CORRECTION 2 : Cleanup Robuste ---
        [TestCleanup]

        public override void Cleanup()
        {
            var samples = GetSampleEntities();
            _context.RemoveRange(samples);
            _context.SaveChanges();
            _context.Dispose();
        }
        // --- HELPER SECURITE ---

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

        // --- ACTIONS ---

        protected override Task<ActionResult<IEnumerable<UserDetailDto>>> ActionGetAll()
        {
            SetupHttpContext(999, "Admin");
            return _controller.GetAll();
        }

        protected override Task<ActionResult<UserDetailDto>> ActionGetById(int id)
        {
            SetupHttpContext(id, "Client");
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

        // --- TESTS SPECIFIQUES ---

        [TestMethod]
        public async Task ChangePassword_ShouldUpdate_WhenSelf()
        {
            // On s'assure d'avoir un contexte propre pour récupérer le user
            _context.ChangeTracker.Clear();

            // On récupère le premier user de test (1001)
            var userId = 1001;
            var user = await _context.Users.FindAsync(userId);

            // Si le seed a échoué silencieusement, on évite le crash null ref
            Assert.IsNotNull(user, "L'utilisateur de test n'a pas été trouvé en BDD.");

            SetupHttpContext(user.Id, "Client");
            var dto = new ChangePasswordDto { NewPassword = "NewPassword123!" };

            var result = await _controller.ChangePassword(user.Id, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        // --- WRAPPER REPO ---
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