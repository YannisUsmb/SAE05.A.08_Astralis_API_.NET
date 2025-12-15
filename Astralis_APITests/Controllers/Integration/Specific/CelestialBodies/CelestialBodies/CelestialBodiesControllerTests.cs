using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class CelestialBodiesControllerTests
        : CrudControllerTests<CelestialBodiesController, CelestialBody, CelestialBodyListDto, CelestialBodyListDto, CelestialBodyCreateDto, CelestialBodyUpdateDto, int>
    {
        private int _targetTypeId;

        protected override CelestialBodiesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var manager = new CelestialBodyManager(context);
            var controller = new CelestialBodiesController(manager, mapper);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "999"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Name, "AdminUser")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        protected override List<CelestialBody> GetSampleEntities()
        {

            var typeTarget = _context.CelestialBodyTypes.FirstOrDefault(t => t.Label == "Type_Target_Test");
            if (typeTarget == null)
            {
                typeTarget = new CelestialBodyType
                {
                    Label = "Type_Target_Test",
                    Description = "Description requise"
                };
                _context.CelestialBodyTypes.Add(typeTarget);
                _context.SaveChanges();
            }
            _targetTypeId = typeTarget.Id;

            var typeNoise = _context.CelestialBodyTypes.FirstOrDefault(t => t.Label == "Type_Noise_Test");
            if (typeNoise == null)
            {
                typeNoise = new CelestialBodyType
                {
                    Label = "Type_Noise_Test",
                    Description = "Description requise"
                };
                _context.CelestialBodyTypes.Add(typeNoise);
                _context.SaveChanges();
            }

            var body1 = _context.CelestialBodies.FirstOrDefault(c => c.Name == "Body_Target_Alpha");
            if (body1 == null)
            {
                body1 = new CelestialBody
                {
                    Name = "Body_Target_Alpha",
                    CelestialBodyTypeId = typeTarget.Id,
                    Alias = "The Alpha"
                };
            }

            var body2 = _context.CelestialBodies.FirstOrDefault(c => c.Name == "Body_Noise_Beta");
            if (body2 == null)
            {
                body2 = new CelestialBody
                {
                    Name = "Body_Noise_Beta",
                    CelestialBodyTypeId = typeNoise.Id,
                    Alias = "The Beta"
                };
            }
            return new List<CelestialBody> { body1, body2 };
        }


        protected override int GetIdFromEntity(CelestialBody entity) => entity.Id;
        protected override int GetIdFromDto(CelestialBodyListDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override CelestialBodyCreateDto GetValidCreateDto()
        {
            return new CelestialBodyCreateDto
            {
                Name = "New Celestial Body",
                CelestialBodyTypeId = _targetTypeId
            };
        }

        protected override CelestialBodyUpdateDto GetValidUpdateDto(CelestialBody entityToUpdate)
        {
            return new CelestialBodyUpdateDto
            {
                Name = entityToUpdate.Name + " Updated",
                Alias = "Updated Alias"
            };
        }

        protected override void SetIdInUpdateDto(CelestialBodyUpdateDto dto, int id)
        {
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            // Given
            var createDto = GetValidCreateDto();

            // When
            var actionResult = await _controller.Post(createDto);

            // Then
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult), "Le contrôleur devrait bloquer la création directe.");
        }

        [TestMethod]
        public async Task Search_ByName_ShouldReturnMatchingBody()
        {
            // Given
            var filter = new CelestialBodyFilterDto { SearchText = "Target_Alpha" };

            // When
            var result = await _controller.Search(filter);

            // Then
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var items = okResult.Value as IEnumerable<CelestialBodyListDto>;
            Assert.IsTrue(items.Any(c => c.Name == "Body_Target_Alpha"));
        }

        [TestMethod]
        public async Task Search_ByType_ShouldReturnSpecificBody()
        {
            // Given
            var filter = new CelestialBodyFilterDto { CelestialBodyTypeIds = new List<int> { _targetTypeId } };

            // When
            var result = await _controller.Search(filter);

            // Then
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var items = okResult.Value as IEnumerable<CelestialBodyListDto>;
            Assert.IsTrue(items.Any(c => c.Name == "Body_Target_Alpha"));
            Assert.IsFalse(items.Any(c => c.Name == "Body_Noise_Beta"));
        }
    }
}