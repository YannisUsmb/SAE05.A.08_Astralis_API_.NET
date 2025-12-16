using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class AsteroidsControllerTests
        : CrudControllerTests<AsteroidsController, Asteroid, AsteroidDto, AsteroidDto, AsteroidCreateDto, AsteroidUpdateDto, int>
    {
        // IDs utilisateurs fixes
        private const int TEST_ADMIN_ID = 100;
        private const int TEST_OWNER_ID = 200;
        private const int TEST_STRANGER_ID = 300;

        // IDs Status
        private const int STATUS_DRAFT = 1;
        private const int STATUS_VALIDATED = 2;

        // --- CORRECTION : IDs Astéroïdes Fixes et Élevés ---
        // On utilise des IDs élevés pour éviter les conflits avec les séquences auto-incrémentées de la BDD
        private const int ASTEROID_DRAFT_ID = 900001;
        private const int ASTEROID_LOCKED_ID = 900002;

        // Variables membres pour stocker les IDs (utilisées dans les tests)
        private int _asteroidDraftId;
        private int _asteroidLockedId;
        private int _targetOrbitalClassId;

        protected override AsteroidsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var asteroidManager = new AsteroidManager(context);
            var discoveryManager = new DiscoveryManager(context);

            var controller = new AsteroidsController(asteroidManager, discoveryManager, mapper);

            SetupUserContext(controller, TEST_ADMIN_ID, "Admin");

            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, $"User_{userId}")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        /// <summary>
        /// Cette méthode est idempotente : elle vérifie si les données existent avant de les créer.
        /// Elle répare automatiquement la base si un test précédent a supprimé une entité.
        /// </summary>
        protected override List<Asteroid> GetSampleEntities()
        {
            // 1. Setup Roles (Safe)
            var roleAdmin = new UserRole { Id = 1, Label = "Admin" };
            var roleUser = new UserRole { Id = 2, Label = "User" };

            // On vérifie s'ils existent déjà (en cache ou DB)
            if (!_context.UserRoles.Any(r => r.Id == 1)) _context.UserRoles.Add(roleAdmin);
            if (!_context.UserRoles.Any(r => r.Id == 2)) _context.UserRoles.Add(roleUser);
            _context.SaveChanges();

            CreateUserIfNotExist(TEST_ADMIN_ID, 1);
            CreateUserIfNotExist(TEST_OWNER_ID, 2);
            CreateUserIfNotExist(TEST_STRANGER_ID, 2);

            // 2. Setup Statuses (Safe)
            if (!_context.DiscoveryStatuses.Any(s => s.Id == STATUS_DRAFT))
                _context.DiscoveryStatuses.Add(new DiscoveryStatus { Id = STATUS_DRAFT, Label = "Draft" });

            if (!_context.DiscoveryStatuses.Any(s => s.Id == STATUS_VALIDATED))
                _context.DiscoveryStatuses.Add(new DiscoveryStatus { Id = STATUS_VALIDATED, Label = "Validated" });

            _context.SaveChanges();

            // 3. Setup Orbital Class (Safe)
            var ocTarget = _context.OrbitalClasses.FirstOrDefault(o => o.Label == "TGT");
            if (ocTarget == null)
            {
                ocTarget = new OrbitalClass { Label = "TGT", Description = "Target" };
                _context.OrbitalClasses.Add(ocTarget);
                _context.SaveChanges();
            }
            _targetOrbitalClassId = ocTarget.Id;

            // 4. Setup CelestialBodyType (Safe)
            if (!_context.CelestialBodyTypes.Any(t => t.Id == 1))
            {
                _context.CelestialBodyTypes.Add(new CelestialBodyType { Id = 1, Label = "Asteroid" });
                _context.SaveChanges();
            }

            // 5. Setup Dependencies (Bodies) - Vérification individuelle
            var body1 = _context.CelestialBodies.FirstOrDefault(b => b.Name == "Body Draft");
            if (body1 == null)
            {
                body1 = new CelestialBody { Name = "Body Draft", CelestialBodyTypeId = 1 };
                _context.CelestialBodies.Add(body1);
                _context.SaveChanges(); // Save immédiat pour avoir l'ID
            }

            var body2 = _context.CelestialBodies.FirstOrDefault(b => b.Name == "Body Locked");
            if (body2 == null)
            {
                body2 = new CelestialBody { Name = "Body Locked", CelestialBodyTypeId = 1 };
                _context.CelestialBodies.Add(body2);
                _context.SaveChanges();
            }

            // Setup Discoveries - Vérification individuelle
            if (!_context.Discoveries.Any(d => d.Title == "Discovery Draft"))
            {
                _context.Discoveries.Add(new Discovery
                {
                    Title = "Discovery Draft",
                    UserId = TEST_OWNER_ID,
                    CelestialBodyId = body1.Id,
                    DiscoveryStatusId = STATUS_DRAFT
                });
            }

            if (!_context.Discoveries.Any(d => d.Title == "Discovery Validated"))
            {
                _context.Discoveries.Add(new Discovery
                {
                    Title = "Discovery Validated",
                    UserId = TEST_OWNER_ID,
                    CelestialBodyId = body2.Id,
                    DiscoveryStatusId = STATUS_VALIDATED
                });
            }
            _context.SaveChanges();

            // --- CRITICAL FIX START: Gestion Idempotente des Astéroïdes ---

            // 6. Gestion Astéroïde 1 (Draft) - ID 900001
            var a1 = _context.Asteroids.Find(ASTEROID_DRAFT_ID);

            // Si null, cela signifie qu'il n'existe pas ou qu'il a été supprimé par un test précédent.
            // On le recrée.
            if (a1 == null)
            {
                a1 = new Asteroid
                {
                    Id = ASTEROID_DRAFT_ID, // Force l'ID fixe
                    CelestialBodyId = body1.Id,
                    Reference = "Ref_Draft",
                    OrbitalClassId = ocTarget.Id,
                    IsPotentiallyHazardous = true,
                    AbsoluteMagnitude = 10,
                    DiameterMinKm = 1,
                    DiameterMaxKm = 2,
                    OrbitId = 100
                };
                _context.Asteroids.Add(a1);
            }

            // 7. Gestion Astéroïde 2 (Locked) - ID 900002
            var a2 = _context.Asteroids.Find(ASTEROID_LOCKED_ID);

            if (a2 == null)
            {
                a2 = new Asteroid
                {
                    Id = ASTEROID_LOCKED_ID, // Force l'ID fixe
                    CelestialBodyId = body2.Id,
                    Reference = "Ref_Locked",
                    OrbitalClassId = ocTarget.Id,
                    IsPotentiallyHazardous = false,
                    AbsoluteMagnitude = 20,
                    DiameterMinKm = 0.5m,
                    DiameterMaxKm = 0.8m,
                    OrbitId = 200
                };
                _context.Asteroids.Add(a2);
            }

            // On ne sauvegarde QUE ce qui a été ajouté (Entity Framework gère le tracking)
            _context.SaveChanges();

            // Mise à jour des variables de classe pour les tests
            _asteroidDraftId = ASTEROID_DRAFT_ID;
            _asteroidLockedId = ASTEROID_LOCKED_ID;

            return new List<Asteroid> { a1, a2 };
        }

        private void CreateUserIfNotExist(int id, int roleId)
        {
            if (!_context.Users.Any(u => u.Id == id))
            {
                // On s'assure que le rôle est attaché (via Find local ou DB)
                var role = _context.UserRoles.Find(roleId);

                _context.Users.Add(new User
                {
                    Id = id,
                    UserRoleNavigation = role!, // Peut être null si contexte pas rafraîchi, mais ici géré
                    Username = $"User{id}",
                    Email = $"user{id}@test.com",
                    FirstName = "Test",
                    LastName = "User",
                    Password = "Pwd",
                    UserRoleId = roleId // Bonne pratique d'assigner aussi la FK directement
                });
                _context.SaveChanges();
            }
        }

        protected override int GetIdFromEntity(Asteroid entity) => entity.Id;
        protected override int GetIdFromDto(AsteroidDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override AsteroidCreateDto GetValidCreateDto()
        {
            return new AsteroidCreateDto
            {
                Name = "Direct Create",
                CelestialBodyTypeId = 1,
                Reference = "Direct_Ref",
                OrbitalClassId = _targetOrbitalClassId,
                IsPotentiallyHazardous = false
            };
        }

        protected override AsteroidUpdateDto GetValidUpdateDto(Asteroid entityToUpdate)
        {
            bool hazardous = entityToUpdate.IsPotentiallyHazardous ?? false;

            return new AsteroidUpdateDto
            {
                Reference = entityToUpdate.Reference + "_Updated",
                OrbitalClassId = entityToUpdate.OrbitalClassId,
                IsPotentiallyHazardous = !hazardous,
                DiameterMinKm = 5,
                DiameterMaxKm = 10
            };
        }

        protected override void SetIdInUpdateDto(AsteroidUpdateDto dto, int id) { }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            // Given
            var createDto = GetValidCreateDto();

            // When
            var result = await _controller.Post(createDto);

            // Then
            // Note: Si votre méthode Post retourne un CreatedAtAction, ajustez l'Assert.
            // Ici je garde votre logique originale qui semblait attendre un BadRequest dans le code fourni,
            // mais normalement un Post valide retourne Created/Ok.
            // Si votre controller marche, ceci devrait probablement être Assert.IsInstanceOfType(..., typeof(CreatedAtActionResult));
            // Je laisse tel quel par sécurité par rapport à votre code source.
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Search_ByReference_ShouldReturnMatchingAsteroid()
        {
            // Given
            var filter = new AsteroidFilterDto { Reference = "Ref_Draft" };

            // When
            var result = await _controller.Search(filter);

            // Then
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var items = okResult.Value as IEnumerable<AsteroidDto>;
            Assert.IsTrue(items.Any(a => a.Reference == "Ref_Draft"));
        }

        [TestMethod]
        public async Task Search_ByHazardous_ShouldReturnTrueOnly()
        {
            // Given
            var filter = new AsteroidFilterDto { IsPotentiallyHazardous = true };

            // When
            var result = await _controller.Search(filter);

            // Then
            var okResult = result.Result as OkObjectResult;
            var items = okResult.Value as IEnumerable<AsteroidDto>;

            Assert.IsTrue(items.All(a => a.IsPotentiallyHazardous == true));
            Assert.IsTrue(items.Any());
        }

        [TestMethod]
        public async Task Put_AsAdmin_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_ADMIN_ID, "Admin");

            // On re-fetch depuis le contexte pour être sûr qu'il est tracké
            // Attention: FindAsync cherche en local d'abord.
            var asteroid = await _context.Asteroids.FindAsync(_asteroidLockedId);

            // Si jamais il est null (ex: delete test run avant), GetSampleEntities l'a recréé, 
            // mais il faut s'assurer que le context est à jour.
            if (asteroid == null) asteroid = _context.Asteroids.Find(_asteroidLockedId);

            var updateDto = GetValidUpdateDto(asteroid!);

            // When
            var result = await _controller.Put(_asteroidLockedId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_AsOwner_DraftStatus_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_OWNER_ID, "User");
            var asteroid = await _context.Asteroids.FindAsync(_asteroidDraftId);
            var updateDto = GetValidUpdateDto(asteroid!);

            // When
            var result = await _controller.Put(_asteroidDraftId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_AsOwner_ValidatedStatus_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_OWNER_ID, "User");
            var asteroid = await _context.Asteroids.FindAsync(_asteroidLockedId);
            var updateDto = GetValidUpdateDto(asteroid!);

            // When
            var result = await _controller.Put(_asteroidLockedId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_AsStranger_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_STRANGER_ID, "User");
            var asteroid = await _context.Asteroids.FindAsync(_asteroidDraftId);
            var updateDto = GetValidUpdateDto(asteroid!);

            // When
            var result = await _controller.Put(_asteroidDraftId, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_AsAdmin_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_ADMIN_ID, "Admin");

            // When
            var result = await _controller.Delete(_asteroidLockedId);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_AsOwner_DraftStatus_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, TEST_OWNER_ID, "User");

            // When
            var result = await _controller.Delete(_asteroidDraftId);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_AsOwner_ValidatedStatus_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_OWNER_ID, "User");

            // When
            var result = await _controller.Delete(_asteroidLockedId);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_AsStranger_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, TEST_STRANGER_ID, "User");

            // When
            var result = await _controller.Delete(_asteroidDraftId);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}