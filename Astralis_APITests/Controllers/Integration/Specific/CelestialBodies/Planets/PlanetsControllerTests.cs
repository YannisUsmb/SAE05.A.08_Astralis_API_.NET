//using Astralis_API.Controllers;
//using Astralis.Shared.DTOs;
//using Astralis_API.Models.EntityFramework;
//using Astralis_API.Models.Repository;
//using AutoMapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Linq;

//namespace Astralis_APITests.Controllers
//{
//    [TestClass]
//    public class PlanetsControllerTests
//    {
//        private Mock<IGalaxyQuasarRepository> _mockRepo;
//        private Mock<IDiscoveryRepository> _mockDiscoveryRepo;
//        private Mock<IMapper> _mockMapper;
//        private PlanetsController _controller;

//        [TestInitialize]
//        public void Init()
//        {
//            _mockRepo = new Mock<IGalaxyQuasarRepository>();
//            _mockDiscoveryRepo = new Mock<IDiscoveryRepository>();
//            _mockMapper = new Mock<IMapper>();

            
//        }

//        // Helper pour configurer l'utilisateur (Admin ou Standard)
//        private void SetupUser(int userId, string role)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
//                new Claim(ClaimTypes.Role, role)
//            };
//            var identity = new ClaimsIdentity(claims, "TestAuth");
//            var principal = new ClaimsPrincipal(identity);

//            _controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext { User = principal }
//            };
//        }

//        // ==========================================
//        // TESTS GET (Read)
//        // ==========================================

//        [TestMethod]
//        public async Task GetAll_ShouldReturnOkWithList()
//        {
//            // GIVEN
//            var entities = new List<GalaxyQuasar> { new GalaxyQuasar { Id = 1 }, new GalaxyQuasar { Id = 2 } };
//            var dtos = new List<GalaxyQuasarDto> { new GalaxyQuasarDto { Id = 1 }, new GalaxyQuasarDto { Id = 2 } };

//            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
//            _mockMapper.Setup(m => m.Map<IEnumerable<GalaxyQuasarDto>>(entities)).Returns(dtos);

//            // WHEN
//            var result = await _controller.GetAll();

//            // THEN
//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult);
//            var returnDtos = okResult.Value as IEnumerable<GalaxyQuasarDto>;
//            Assert.AreEqual(2, returnDtos.Count());
//        }

//        [TestMethod]
//        public async Task GetById_ExistingId_ShouldReturnOk()
//        {
//            // GIVEN
//            int id = 1;
//            var entity = new GalaxyQuasar { Id = id };
//            var dto = new GalaxyQuasarDto { Id = id };

//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
//            _mockMapper.Setup(m => m.Map<GalaxyQuasarDto>(entity)).Returns(dto);

//            // WHEN
//            var result = await _controller.GetById(id);

//            // THEN
//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult);
//            Assert.AreEqual(id, ((GalaxyQuasarDto)okResult.Value).Id);
//        }

//        [TestMethod]
//        public async Task GetById_UnknownId_ShouldReturnNotFound()
//        {
//            // GIVEN
//            int id = 99;
//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((GalaxyQuasar?)null);

//            // WHEN
//            var result = await _controller.GetById(id);

//            // THEN
//            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
//        }

//        [TestMethod]
//        public async Task Search_ShouldReturnOkWithResults()
//        {
//            // GIVEN
//            var filter = new GalaxyQuasarFilterDto { Reference = "Test" };
//            var entities = new List<GalaxyQuasar> { new GalaxyQuasar { Reference = "Test1" } };
//            var dtos = new List<GalaxyQuasarDto> { new GalaxyQuasarDto { Reference = "Test1" } };

//            _mockRepo.Setup(r => r.SearchAsync(
//                filter.Reference, filter.GalaxyQuasarClassIds,
//                null, null, null, null, null, null, null, null, null, null
//            )).ReturnsAsync(entities);


//            // WHEN
//            var result = await _controller.Search(filter);

//            // THEN
//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult);
//        }

//        // ==========================================
//        // TESTS POST (Create) - Doit être bloqué
//        // ==========================================

//        [TestMethod]
//        public async Task Post_ShouldAlwaysReturnBadRequest()
//        {
//            // GIVEN
//            var createDto = new GalaxyQuasarCreateDto();
//            SetupUser(1, "User");

//            // WHEN
//            var result = await _controller.Post(createDto);

//            // THEN
//            // Le contrôleur renvoie explicitement BadRequest pour empêcher la création directe
//            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
//        }

//        // ==========================================
//        // TESTS PUT (Update) - Avec Sécurité
//        // ==========================================

//        [TestMethod]
//        public async Task Put_Admin_ShouldUpdateAndReturnNoContent()
//        {
//            // GIVEN
//            int id = 1;
//            var updateDto = new GalaxyQuasarUpdateDto { Reference = "Updated" };
//            var entity = new GalaxyQuasar { Id = id, CelestialBodyId = 100 };

//            SetupUser(99, "Admin");

//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
//            _mockMapper.Setup(m => m.Map(updateDto, entity)).Returns(entity);

//            // Mock UpdateAsync avec 2 arguments
//            _mockRepo.Setup(r => r.UpdateAsync(entity, It.IsAny<GalaxyQuasar>())).Returns(Task.CompletedTask);

//            // WHEN
//            var result = await _controller.Put(id, updateDto);

//            // THEN
//            Assert.IsInstanceOfType(result, typeof(NoContentResult));
//            _mockRepo.Verify(r => r.UpdateAsync(entity, It.IsAny<GalaxyQuasar>()), Times.Once);
//        }

//        [TestMethod]
//        public async Task Put_Owner_DraftDiscovery_ShouldUpdate()
//        {
//            // GIVEN
//            int id = 1;
//            int userId = 10;
//            var updateDto = new GalaxyQuasarUpdateDto();
//            var entity = new GalaxyQuasar { Id = id, CelestialBodyId = 50 };

//            // Discovery en Draft (Status 1) appartenant au user
//            var discovery = new Discovery { UserId = userId, CelestialBodyId = 50, DiscoveryStatusId = 1 };

//            SetupUser(userId, "User");

//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
//            _mockDiscoveryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Discovery> { discovery });
//            _mockMapper.Setup(m => m.Map(updateDto, entity)).Returns(entity);
//            _mockRepo.Setup(r => r.UpdateAsync(entity, It.IsAny<GalaxyQuasar>())).Returns(Task.CompletedTask);

//            // WHEN
//            var result = await _controller.Put(id, updateDto);

//            // THEN
//            Assert.IsInstanceOfType(result, typeof(NoContentResult));
//        }

//        [TestMethod]
//        public async Task Put_Owner_ValidatedDiscovery_ShouldReturnForbidden()
//        {
//            // GIVEN
//            int id = 1;
//            int userId = 10;
//            var entity = new GalaxyQuasar { Id = id, CelestialBodyId = 50 };

//            // Discovery Validée (Status 2) -> Modification Interdite
//            var discovery = new Discovery { UserId = userId, CelestialBodyId = 50, DiscoveryStatusId = 2 };

//            SetupUser(userId, "User");

//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
//            _mockDiscoveryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Discovery> { discovery });

//            // WHEN
//            var result = await _controller.Put(id, new GalaxyQuasarUpdateDto());

//            // THEN
//            Assert.IsInstanceOfType(result, typeof(ForbidResult));
//            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<GalaxyQuasar>(), It.IsAny<GalaxyQuasar>()), Times.Never);
//        }

//        [TestMethod]
//        public async Task Put_OtherUser_ShouldReturnForbidden()
//        {
//            // GIVEN
//            int id = 1;
//            int ownerId = 10;
//            int hackerId = 666;
//            var entity = new GalaxyQuasar { Id = id, CelestialBodyId = 50 };

//            var discovery = new Discovery { UserId = ownerId, CelestialBodyId = 50, DiscoveryStatusId = 1 };

//            SetupUser(hackerId, "User"); // Ce n'est pas le propriétaire

//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
//            _mockDiscoveryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Discovery> { discovery });

//            // WHEN
//            var result = await _controller.Put(id, new GalaxyQuasarUpdateDto());

//            // THEN
//            Assert.IsInstanceOfType(result, typeof(ForbidResult));
//        }

//        // ==========================================
//        // TESTS DELETE - Avec Sécurité
//        // ==========================================

//        [TestMethod]
//        public async Task Delete_Admin_ShouldDeleteAndReturnNoContent()
//        {
//            // GIVEN
//            int id = 1;
//            var entity = new GalaxyQuasar { Id = id, CelestialBodyId = 100 };

//            SetupUser(99, "Admin");

//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
//            _mockRepo.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

//            // WHEN
//            var result = await _controller.Delete(id);

//            // THEN
//            Assert.IsInstanceOfType(result, typeof(NoContentResult));
//            _mockRepo.Verify(r => r.DeleteAsync(entity), Times.Once);
//        }

//        [TestMethod]
//        public async Task Delete_Owner_DraftDiscovery_ShouldDelete()
//        {
//            // GIVEN
//            int id = 1;
//            int userId = 10;
//            var entity = new GalaxyQuasar { Id = id, CelestialBodyId = 50 };
//            var discovery = new Discovery { UserId = userId, CelestialBodyId = 50, DiscoveryStatusId = 1 };

//            SetupUser(userId, "User");

//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
//            _mockDiscoveryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Discovery> { discovery });
//            _mockRepo.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);

//            // WHEN
//            var result = await _controller.Delete(id);

//            // THEN
//            Assert.IsInstanceOfType(result, typeof(NoContentResult));
//        }

//        [TestMethod]
//        public async Task Delete_Owner_ValidatedDiscovery_ShouldReturnForbidden()
//        {
//            // GIVEN
//            int id = 1;
//            int userId = 10;
//            var entity = new GalaxyQuasar { Id = id, CelestialBodyId = 50 };
//            // Validée -> Suppression interdite
//            var discovery = new Discovery { UserId = userId, CelestialBodyId = 50, DiscoveryStatusId = 2 };

//            SetupUser(userId, "User");

//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
//            _mockDiscoveryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Discovery> { discovery });

//            // WHEN
//            var result = await _controller.Delete(id);

//            // THEN
//            Assert.IsInstanceOfType(result, typeof(ForbidResult));
//            _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<GalaxyQuasar>()), Times.Never);
//        }

//        [TestMethod]
//        public async Task Delete_UnknownId_ShouldReturnNotFound()
//        {
//            // GIVEN
//            int id = 999;
//            SetupUser(1, "Admin");
//            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((GalaxyQuasar?)null);

//            // WHEN
//            var result = await _controller.Delete(id);

//            // THEN
//            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
//        }
//    }
//}