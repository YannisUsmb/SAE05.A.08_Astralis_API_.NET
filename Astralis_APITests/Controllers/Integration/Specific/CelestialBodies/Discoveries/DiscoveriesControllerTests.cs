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
    public class DiscoveriesControllerTests
        : CrudControllerTests<DiscoveriesController, Discovery, DiscoveryDto, DiscoveryDto, DiscoveryCreateDto, DiscoveryUpdateDto, int>
    {
        private const int USER_OWNER_ID = 5001;
        private const int USER_ADMIN_ID = 5002;

        private int _orbitalClassId;
        private int _asteroidTypeId;


        protected override DiscoveriesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var discoveryRepo = new DiscoveryManager(context);
            var asteroidRepo = new AsteroidManager(context);
            var planetRepo = new PlanetManager(context);
            var starRepo = new StarManager(context);
            var cometRepo = new CometManager(context);
            var galaxyRepo = new GalaxyQuasarManager(context);
            var celestialBodyRepo = new CelestialBodyManager(context);

            var controller = new DiscoveriesController(
                discoveryRepo, asteroidRepo, planetRepo, starRepo, cometRepo, galaxyRepo, celestialBodyRepo, mapper
            );

            SetupUserContext(controller, USER_OWNER_ID, "Client");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, "TestUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<Discovery> GetSampleEntities()
        {
            var statusDraft = _context.DiscoveryStatuses.FirstOrDefault(s => s.Id == 1);
            if (statusDraft == null) { statusDraft = new DiscoveryStatus { Id = 1, Label = "Draft" }; _context.DiscoveryStatuses.Add(statusDraft); }

            var statusAccepted = _context.DiscoveryStatuses.FirstOrDefault(s => s.Id == 3);
            if (statusAccepted == null) { statusAccepted = new DiscoveryStatus { Id = 3, Label = "Accepted" }; _context.DiscoveryStatuses.Add(statusAccepted); }

            if (!_context.DiscoveryStatuses.Any(s => s.Id == 4)) _context.DiscoveryStatuses.Add(new DiscoveryStatus { Id = 4, Label = "Declined" });

            var typeAsteroid = _context.CelestialBodyTypes.FirstOrDefault(t => t.Label == "Asteroid");
            if (typeAsteroid == null)
            {
                typeAsteroid = new CelestialBodyType { Label = "Asteroid", Description = "Space Rock" };
                _context.CelestialBodyTypes.Add(typeAsteroid);
            }

            var orbClass = _context.OrbitalClasses.FirstOrDefault(o => o.Label == "TST");
            if (orbClass == null)
            {
                orbClass = new OrbitalClass { Label = "TST", Description = "Test Class" };
                _context.OrbitalClasses.Add(orbClass);
            }

            var roleClient = _context.UserRoles.FirstOrDefault(r => r.Label == "Client");
            if (roleClient == null) { roleClient = new UserRole { Label = "Client" }; _context.UserRoles.Add(roleClient); }

            var roleAdmin = _context.UserRoles.FirstOrDefault(r => r.Label == "Admin");
            if (roleAdmin == null) { roleAdmin = new UserRole { Label = "Admin" }; _context.UserRoles.Add(roleAdmin); }

            _context.SaveChanges();

            _orbitalClassId = orbClass.Id;
            _asteroidTypeId = typeAsteroid.Id;

            var userOwner = _context.Users.FirstOrDefault(u => u.Id == USER_OWNER_ID);
            if (userOwner == null)
            {
                userOwner = new User { Id = USER_OWNER_ID, Username = "Owner", UserRoleId = roleClient.Id, Email = "owner@test.com", FirstName = "O", LastName = "W", Password = "pwd", IsPremium = false };
                _context.Users.Add(userOwner);
            }

            var userAdmin = _context.Users.FirstOrDefault(u => u.Id == USER_ADMIN_ID);
            if (userAdmin == null)
            {
                userAdmin = new User { Id = USER_ADMIN_ID, Username = "Admin", UserRoleId = roleAdmin.Id, Email = "admin@test.com", FirstName = "A", LastName = "D", Password = "pwd", IsPremium = false };
                _context.Users.Add(userAdmin);
            }
            _context.SaveChanges();

            var bodyDraft = _context.CelestialBodies.FirstOrDefault(c => c.Name == "Body_Draft_Discovery");
            if (bodyDraft == null)
            {
                bodyDraft = new CelestialBody { Name = "Body_Draft_Discovery", CelestialBodyTypeId = _asteroidTypeId, Alias = "RefDraft" };
                _context.CelestialBodies.Add(bodyDraft);
            }

            var bodyAccepted = _context.CelestialBodies.FirstOrDefault(c => c.Name == "Body_Accepted_Discovery");
            if (bodyAccepted == null)
            {
                bodyAccepted = new CelestialBody { Name = "Body_Accepted_Discovery", CelestialBodyTypeId = _asteroidTypeId, Alias = "RefAcc" };
                _context.CelestialBodies.Add(bodyAccepted);
            }
            _context.SaveChanges();

            var list = new List<Discovery>();

            var draft = _context.Discoveries.FirstOrDefault(d => d.Title == "Draft Discovery Title");
            if (draft == null)
            {
                draft = new Discovery
                {
                    Title = "Draft Discovery Title",
                    UserId = USER_OWNER_ID,
                    CelestialBodyId = bodyDraft.Id,
                    DiscoveryStatusId = 1,

                    DiscoveryStatusNavigation = statusDraft,
                    UserNavigation = userOwner,
                    CelestialBodyNavigation = bodyDraft
                };
                list.Add(draft);
            }

            var accepted = _context.Discoveries.FirstOrDefault(d => d.Title == "Accepted Discovery Title");
            if (accepted == null)
            {
                accepted = new Discovery
                {
                    Title = "Accepted Discovery Title",
                    UserId = USER_OWNER_ID,
                    CelestialBodyId = bodyAccepted.Id,
                    DiscoveryStatusId = 3,

                    DiscoveryStatusNavigation = statusAccepted,
                    UserNavigation = userOwner,
                    CelestialBodyNavigation = bodyAccepted
                };
                list.Add(accepted);
            }

            return list;
        }

        protected override int GetIdFromEntity(Discovery entity) => entity.Id;
        protected override int GetIdFromDto(DiscoveryDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;
        protected override DiscoveryCreateDto GetValidCreateDto() => new DiscoveryCreateDto { Title = "Generic Post (Should Fail)" };
        protected override DiscoveryUpdateDto GetValidUpdateDto(Discovery entityToUpdate) => new DiscoveryUpdateDto { Title = entityToUpdate.Title + " Updated" };
        protected override void SetIdInUpdateDto(DiscoveryUpdateDto dto, int id) { }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();
            var actionResult = await _controller.Post(createDto);
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public async Task PostAsteroid_ValidSubmission_ShouldCreateDiscoveryAndReturn200()
        {
            // Given
            string uniqueRef = $"AST-{Guid.NewGuid().ToString().Substring(0, 8)}";

            var submission = new DiscoveryAsteroidSubmissionDto
            {
                Title = "New Asteroid Discovery",
                Details = new AsteroidCreateDto
                {
                    Name = "New Found Asteroid",
                    Reference = uniqueRef,
                    CelestialBodyTypeId = _asteroidTypeId,
                    OrbitalClassId = _orbitalClassId,
                    IsPotentiallyHazardous = false,
                    DiameterMinKm = 1,
                    DiameterMaxKm = 2,
                    AbsoluteMagnitude = 10,
                    SemiMajorAxis = 1,
                    Inclination = 10
                }
            };

            // When
            var actionResult = await _controller.PostAsteroid(submission);

            // Then
            var okResult = actionResult.Result as OkObjectResult;

            if (okResult == null && actionResult.Result is BadRequestObjectResult badRequest)
            {
                Assert.Fail($"Le test a échoué avec une erreur 400 : {badRequest.Value}");
            }

            Assert.IsNotNull(okResult, "Devrait retourner 200 OK");
            var dto = okResult.Value as DiscoveryDto;
            Assert.IsNotNull(dto);
            Assert.AreEqual("New Asteroid Discovery", dto.Title);
            var dbDiscovery = _context.Discoveries.Find(dto.Id);
            Assert.IsNotNull(dbDiscovery);
            Assert.AreEqual(1, dbDiscovery.DiscoveryStatusId);
        }

        [TestMethod]
        public async Task Put_UpdateTitle_OnDraft_ShouldSuccess()
        {
            var draftId = _context.Discoveries.First(d => d.Title == "Draft Discovery Title").Id;
            var updateDto = new DiscoveryUpdateDto { Title = "Updated Draft Title" };

            var actionResult = await _controller.Put(draftId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
            _context.Entry(_context.Discoveries.Find(draftId)).Reload();
            Assert.AreEqual("Updated Draft Title", _context.Discoveries.Find(draftId).Title);
        }

        [TestMethod]
        public async Task Put_UpdateTitle_OnAccepted_ShouldFail()
        {
            var acceptedId = _context.Discoveries.First(d => d.Title == "Accepted Discovery Title").Id;
            var updateDto = new DiscoveryUpdateDto { Title = "Trying to hack" };

            var actionResult = await _controller.Put(acceptedId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task ModerateDiscovery_AsAdmin_ShouldUpdateStatus()
        {
            var draftId = _context.Discoveries.First(d => d.Title == "Draft Discovery Title").Id;
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");

            var moderationDto = new DiscoveryModerationDto { DiscoveryStatusId = 3, AliasStatusId = 3 };

            var actionResult = await _controller.ModerateDiscovery(draftId, moderationDto);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
            _context.Entry(_context.Discoveries.Find(draftId)).Reload();
            Assert.AreEqual(3, _context.Discoveries.Find(draftId).DiscoveryStatusId);
        }

        [TestMethod]
        public async Task Delete_AsOwner_OnDraft_ShouldSuccess()
        {
            var draftId = _context.Discoveries.First(d => d.Title == "Draft Discovery Title").Id;
            var actionResult = await _controller.Delete(draftId);
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
            Assert.IsNull(_context.Discoveries.Find(draftId));
        }
    }
}