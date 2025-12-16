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
    public class CitiesControllerTests
        : CrudControllerTests<CitiesController, City, CityDto, CityDto, CityCreateDto, CityCreateDto, int>
    {
        private const int TEST_COUNTRY_FR_ID = 101;
        private int _city1Id;
        private int _testCountryId;

        protected override CitiesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var controller = new CitiesController(new CityManager(context), mapper);
            SetupUserContext(controller);
            return controller;
        }

        private void SetupUserContext(ControllerBase controller)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<City> GetSampleEntities()
        {
            var country = _context.Countries.FirstOrDefault(x => x.Id == TEST_COUNTRY_FR_ID);
            if (country == null)
            {
                country = new Country { Id = TEST_COUNTRY_FR_ID, Name = "France" };
                _context.Countries.Add(country);
            }

            _context.SaveChanges();
            _testCountryId = country.Id;

            var cities = new List<City>();

            var c1 = new City
            {
                Name = "PARIS 01",
                PostCode = "75001",
                CountryId = country.Id,
                Latitude = 48.8626305M,
                Longitude = 2.3362934M,
                CountryNavigation = country
            };

            var existingC1 = _context.Cities.FirstOrDefault(c => c.Name == c1.Name && c.PostCode == c1.PostCode);
            if (existingC1 == null)
            {
                cities.Add(c1);
            }
            else
            {
                _city1Id = existingC1.Id;
                existingC1.CountryNavigation = country;
            }

            var c2 = new City
            {
                Name = "LYON 01",
                PostCode = "69001",
                CountryId = country.Id,
                Latitude = 45.7699284M,
                Longitude = 4.8292246M,
                CountryNavigation = country
            };

            var existingC2 = _context.Cities.FirstOrDefault(c => c.Name == c2.Name && c.PostCode == c2.PostCode);
            if (existingC2 == null)
            {
                cities.Add(c2);
            }
            else
            {
                existingC2.CountryNavigation = country;
            }

            return cities;
        }

        protected override int GetIdFromEntity(City entity)
        {
            return entity.Id;
        }

        protected override int GetIdFromDto(CityDto dto)
        {
            return dto.Id;
        }

        protected override int GetNonExistingId()
        {
            return 999999;
        }

        protected override CityCreateDto GetValidCreateDto()
        {
            var countryId = _testCountryId;
            if (countryId == 0)
            {
                var country = _context.Countries.FirstOrDefault();
                if (country != null)
                {
                    countryId = country.Id;
                }
                else
                {
                    var newC = new Country { Name = "Fallback Country" };
                    _context.Countries.Add(newC);
                    _context.SaveChanges();
                    countryId = newC.Id;
                }
            }

            return new CityCreateDto
            {
                Name = "Marseille",
                PostCode = "13000",
                CountryId = countryId,
                Latitude = 43.2965M,
                Longitude = 5.3698M
            };
        }

        protected override CityCreateDto GetValidUpdateDto(City entityToUpdate)
        {
            return new CityCreateDto
            {
                Name = entityToUpdate.Name + " Updated",
                PostCode = entityToUpdate.PostCode,
                CountryId = entityToUpdate.CountryId,
                Latitude = entityToUpdate.Latitude,
                Longitude = entityToUpdate.Longitude
            };
        }

        protected override void SetIdInUpdateDto(CityCreateDto dto, int id)
        {

        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();
            var actionResult = await _controller.Post(createDto);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultDto = okResult.Value as CityDto;
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(createDto.Name, resultDto.Name);
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            int idToTest = _city1Id;
            if (idToTest == 0) idToTest = _context.Cities.First().Id;

            var actionResult = await _controller.GetById(idToTest);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "GetById should return OkObjectResult");
            var dto = okResult.Value as CityDto;
            Assert.IsNotNull(dto);
            Assert.AreEqual(idToTest, dto.Id);
        }

        [TestMethod]
        public async Task Put_ValidObject_ShouldUpdateAndReturn204()
        {
            int idToTest = _city1Id;
            if (idToTest == 0) idToTest = _context.Cities.First().Id;

            _context.ChangeTracker.Clear();
            var entity = await _context.Cities.FindAsync(idToTest);
            var updateDto = GetValidUpdateDto(entity!);

            var actionResult = await _controller.Put(idToTest, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updatedEntity = await _context.Cities.FindAsync(idToTest);
            Assert.IsTrue(updatedEntity!.Name.Contains("Updated"));
        }

        [TestMethod]
        public async Task Put_NonExistingId_ShouldReturn404()
        {
            var id = GetNonExistingId();

            // On a besoin d'un DTO valide, peu importe les données dedans
            var updateDto = GetValidCreateDto();

            var actionResult = await _controller.Put(id, updateDto);

            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ExistingId_ShouldDeleteAndReturn204()
        {
            int idToTest = _city1Id;
            if (idToTest == 0) idToTest = _context.Cities.First().Id;

            _context.ChangeTracker.Clear();

            var actionResult = await _controller.Delete(idToTest);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            var deletedEntity = await _context.Cities.FindAsync(idToTest);
            Assert.IsNull(deletedEntity);
        }
    }
}