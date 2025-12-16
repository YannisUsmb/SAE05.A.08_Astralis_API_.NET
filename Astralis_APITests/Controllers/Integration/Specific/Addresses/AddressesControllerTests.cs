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
    public class AddressesControllerTests
        : CrudControllerTests<AddressesController, Address, AddressDto, AddressDto, AddressCreateDto, AddressUpdateDto, int>
    {
        private const int TEST_COUNTRY_FR_ID = 101;
        private const int TEST_CITY_PARIS_ID = 201;

        private int _address1Id;
        private int _testCityId;

        protected override AddressesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var controller = new AddressesController(new AddressManager(context), mapper);
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

        protected override List<Address> GetSampleEntities()
        {
            var country = GetOrCreateCountry(TEST_COUNTRY_FR_ID, "France", "FR");
            var city = GetOrCreateCity(TEST_CITY_PARIS_ID, "Paris", "75000", country.Id);

            city.CountryNavigation = country;

            _testCityId = city.Id;
            _context.SaveChanges();

            var addresses = new List<Address>();

            var a1 = new Address
            {
                StreetNumber = "123",
                StreetAddress = "Champs Elysées",
                CityId = city.Id,
                CityNavigation = city
            };

            var existingA1 = _context.Addresses.FirstOrDefault(a => a.StreetAddress == a1.StreetAddress && a.StreetNumber == a1.StreetNumber);
            if (existingA1 == null)
            {
                addresses.Add(a1);
            }
            else
            {
                existingA1.CityNavigation = city;
                _address1Id = existingA1.Id;
            }

            var a2 = new Address
            {
                StreetNumber = "10",
                StreetAddress = "Rue de Rivoli",
                CityId = city.Id,
                CityNavigation = city
            };

            var existingA2 = _context.Addresses.FirstOrDefault(a => a.StreetAddress == a2.StreetAddress && a.StreetNumber == a2.StreetNumber);
            if (existingA2 == null)
            {
                addresses.Add(a2);
            }
            else
            {
                existingA2.CityNavigation = city;
            }

            return addresses;
        }

        private Country GetOrCreateCountry(int id, string name, string code)
        {
            var c = _context.Countries.FirstOrDefault(x => x.Id == id);
            if (c == null)
            {
                c = new Country { Id = id, Name = name };
                _context.Countries.Add(c);
                _context.SaveChanges();
            }
            return c;
        }

        private City GetOrCreateCity(int id, string name, string zip, int countryId)
        {
            var c = _context.Cities.FirstOrDefault(x => x.Id == id);
            if (c == null)
            {
                c = new City { Id = id, Name = name, PostCode = zip, CountryId = countryId };
                _context.Cities.Add(c);
                _context.SaveChanges();
            }
            return c;
        }

        protected override int GetIdFromEntity(Address entity)
        {
            return entity.Id;
        }

        protected override int GetIdFromDto(AddressDto dto)
        {
            return dto.Id;
        }

        protected override int GetNonExistingId()
        {
            return 9999999;
        }

        protected override AddressCreateDto GetValidCreateDto()
        {
            return new AddressCreateDto
            {
                StreetNumber = "15",
                StreetAddress = "Boulevard Haussmann",
                CityId = _testCityId
            };
        }

        protected override AddressUpdateDto GetValidUpdateDto(Address entityToUpdate)
        {
            return new AddressUpdateDto
            {
                StreetNumber = "999",
                StreetAddress = entityToUpdate.StreetAddress + " Updated",
                CityId = entityToUpdate.CityId
            };
        }

        protected override void SetIdInUpdateDto(AddressUpdateDto dto, int id)
        {
            // Vide car le DTO ne contient pas d'ID
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            // Given
            var createDto = GetValidCreateDto();

            // When
            var actionResult = await _controller.Post(createDto);

            // Then
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultDto = okResult.Value as AddressDto;
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(createDto.StreetAddress, resultDto.StreetAddress);
            Assert.AreEqual(createDto.StreetNumber, resultDto.StreetNumber);
        }

        [TestMethod]
        public async Task Put_ValidObject_ShouldUpdateAndReturn204()
        {
            // Given
            if (_address1Id == 0)
            {
                var existing = _context.Addresses.FirstOrDefault(a => a.StreetAddress == "Champs Elysées");
                if (existing != null)
                {
                    _address1Id = existing.Id;
                }
            }

            var entity = await _context.Addresses.FindAsync(_address1Id);
            var updateDto = GetValidUpdateDto(entity!);

            // When
            var actionResult = await _controller.Put(_address1Id, updateDto);

            // Then
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updatedEntity = await _context.Addresses.FindAsync(_address1Id);
            Assert.AreEqual("999", updatedEntity!.StreetNumber);
            Assert.IsTrue(updatedEntity.StreetAddress.Contains("Updated"));
        }
    }
}