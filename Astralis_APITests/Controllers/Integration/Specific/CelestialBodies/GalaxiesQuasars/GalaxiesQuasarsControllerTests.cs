using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class GalaxyQuasarControllerTests
        : CrudControllerTests<GalaxyQuasarsController, GalaxyQuasar, GalaxyQuasarDto, GalaxyQuasarDto, GalaxyQuasarCreateDto, GalaxyQuasarUpdateDto, int>
    {
        private const int USER_OWNER_ID = 5001;
        private const int USER_ADMIN_ID = 5002;
        private const int USER_HACKER_ID = 6666;

        private const string REF_DRAFT = "GQ-Draft";
        private const string REF_ACCEPTED = "GQ-Accepted";
        private const string REF_OTHER = "GQ-Other";

        private int _galaxyDraftId;
        private int _galaxyAcceptedId;
        private int _galaxyOtherUserId;


        protected override GalaxyQuasarsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var galaxyRepo = new GalaxyQuasarManager(context);
            var discoveryRepo = new DiscoveryManager(context);

            var controller = new GalaxyQuasarsController(galaxyRepo, discoveryRepo, mapper);
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


        protected override List<GalaxyQuasar> GetSampleEntities()
        {
            var roleClient = GetOrCreateRole("Client");
            var roleAdmin = GetOrCreateRole("Admin");

            CreateUserIfNotExist(USER_OWNER_ID, "Owner", roleClient.Id);
            CreateUserIfNotExist(USER_ADMIN_ID, "Admin", roleAdmin.Id);
            CreateUserIfNotExist(USER_HACKER_ID, "Hacker", roleClient.Id);

            var statusDraft = GetOrCreateStatus(1, "Draft");
            var statusAccepted = GetOrCreateStatus(3, "Accepted");

            var typeGalaxy = GetOrCreateBodyType("Galaxy");
            var classSpiral = GetOrCreateGalaxyClass("Spiral");

            _context.SaveChanges();

            var list = new List<GalaxyQuasar>();

            var gqDraft = PrepareGalaxyWithDiscovery(REF_DRAFT, "Body_GQ_Draft", typeGalaxy.Id, USER_OWNER_ID, statusDraft.Id, classSpiral.Id);
            list.Add(gqDraft);

            var gqAccepted = PrepareGalaxyWithDiscovery(REF_ACCEPTED, "Body_GQ_Accepted", typeGalaxy.Id, USER_OWNER_ID, statusAccepted.Id, classSpiral.Id);
            list.Add(gqAccepted);

            var gqOther = PrepareGalaxyWithDiscovery(REF_OTHER, "Body_GQ_Other", typeGalaxy.Id, USER_HACKER_ID, statusDraft.Id, classSpiral.Id);
            list.Add(gqOther);

            return list;
        }

        private GalaxyQuasar PrepareGalaxyWithDiscovery(string reference, string bodyName, int typeId, int userId, int statusId, int classId)
        {
            var body = _context.CelestialBodies.FirstOrDefault(b => b.Name == bodyName);
            if (body == null)
            {
                body = new CelestialBody { Name = bodyName, CelestialBodyTypeId = typeId, Alias = reference };
                _context.CelestialBodies.Add(body);
                _context.SaveChanges();
            }

            if (!_context.Discoveries.Any(d => d.CelestialBodyId == body.Id))
            {
                var disc = new Discovery
                {
                    Title = $"Disc {reference}",
                    UserId = userId,
                    CelestialBodyId = body.Id,
                    DiscoveryStatusId = statusId
                };
                _context.Discoveries.Add(disc);
                _context.SaveChanges();
            }

            return new GalaxyQuasar
            {
                Reference = reference,
                CelestialBodyId = body.Id,
                GalaxyQuasarClassId = classId
            };
        }

        private UserRole GetOrCreateRole(string label)
        {
            var r = _context.UserRoles.FirstOrDefault(x => x.Label == label);
            if (r == null) { r = new UserRole { Label = label }; _context.UserRoles.Add(r); }
            return r;
        }

        private void CreateUserIfNotExist(int id, string name, int roleId)
        {
            if (!_context.Users.Any(u => u.Id == id))
                _context.Users.Add(new User { Id = id, Username = name, UserRoleId = roleId, Email = $"{name}@test.com", FirstName = name, LastName = "T", Password = "pwd", IsPremium = false });
        }

        private DiscoveryStatus GetOrCreateStatus(int id, string label)
        {
            var s = _context.DiscoveryStatuses.FirstOrDefault(x => x.Id == id);
            if (s == null) { s = new DiscoveryStatus { Id = id, Label = label }; _context.DiscoveryStatuses.Add(s); }
            return s;
        }

        private CelestialBodyType GetOrCreateBodyType(string label)
        {
            var t = _context.CelestialBodyTypes.FirstOrDefault(x => x.Label == label);
            if (t == null) { t = new CelestialBodyType { Label = label }; _context.CelestialBodyTypes.Add(t); }
            return t;
        }

        private GalaxyQuasarClass GetOrCreateGalaxyClass(string label)
        {
            var c = _context.GalaxyQuasarClasses.FirstOrDefault(x => x.Label == label);
            if (c == null) { c = new GalaxyQuasarClass { Label = label, Description = "Test Class" }; _context.GalaxyQuasarClasses.Add(c); }
            return c;
        }

        protected override int GetIdFromEntity(GalaxyQuasar entity) => entity.Id;
        protected override int GetIdFromDto(GalaxyQuasarDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;
        protected override GalaxyQuasarCreateDto GetValidCreateDto() => new GalaxyQuasarCreateDto { Reference = "Should Fail" };
        protected override void SetIdInUpdateDto(GalaxyQuasarUpdateDto dto, int id) { }

        protected override GalaxyQuasarUpdateDto GetValidUpdateDto(GalaxyQuasar entityToUpdate)
        {
            return new GalaxyQuasarUpdateDto
            {
                Reference = entityToUpdate.Reference + " UPDATED",
                GalaxyQuasarClassId = entityToUpdate.GalaxyQuasarClassId
            };
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();
            var actionResult = await _controller.Post(createDto);
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Delete_ExistingId_ShouldDeleteAndReturn204()
        {
            await RefreshIds();
            await base.Delete_ExistingId_ShouldDeleteAndReturn204();
        }

        private async Task RefreshIds()
        {
            var draft = await _context.GalaxiesQuasars.FirstOrDefaultAsync(g => g.Reference == REF_DRAFT);
            if (draft != null) _galaxyDraftId = draft.Id;

            var accepted = await _context.GalaxiesQuasars.FirstOrDefaultAsync(g => g.Reference == REF_ACCEPTED);
            if (accepted != null) _galaxyAcceptedId = accepted.Id;

            var other = await _context.GalaxiesQuasars.FirstOrDefaultAsync(g => g.Reference == REF_OTHER);
            if (other != null) _galaxyOtherUserId = other.Id;
        }

        [TestMethod]
        public async Task Put_Owner_AcceptedDiscovery_ShouldReturnForbidden()
        {
            await RefreshIds();
            var updateDto = new GalaxyQuasarUpdateDto { Reference = "Hacked Update", GalaxyQuasarClassId = 1 };
            var actionResult = await _controller.Put(_galaxyAcceptedId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldReturnForbidden()
        {
            await RefreshIds();
            var updateDto = new GalaxyQuasarUpdateDto { Reference = "Stolen Update", GalaxyQuasarClassId = 1 };
            var actionResult = await _controller.Put(_galaxyOtherUserId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_Admin_ShouldAlwaysSuccess()
        {
            await RefreshIds();
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");

            var currentEntity = await _context.GalaxiesQuasars.AsNoTracking().FirstOrDefaultAsync(x => x.Id == _galaxyAcceptedId);

            var updateDto = new GalaxyQuasarUpdateDto
            {
                Reference = "Admin Override",
                GalaxyQuasarClassId = currentEntity.GalaxyQuasarClassId
            };

            var actionResult = await _controller.Put(_galaxyAcceptedId, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updatedEntity = await _context.GalaxiesQuasars.FindAsync(_galaxyAcceptedId);
            Assert.AreEqual("Admin Override", updatedEntity.Reference);
        }

        [TestMethod]
        public async Task Delete_Owner_AcceptedDiscovery_ShouldReturnForbidden()
        {
            await RefreshIds();
            var actionResult = await _controller.Delete(_galaxyAcceptedId);
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }
    }
}