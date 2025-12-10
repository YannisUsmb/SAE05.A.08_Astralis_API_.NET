using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class AddressesControllerTests
        : CrudControllerTests<AddressesController, Address, AddressDto, AddressDto, AddressCreateDto, AddressUpdateDto, int>
    {
        protected override AddressesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new AddressesController(new AddressManager(context), mapper);
        }

        protected override List<Address> GetSampleEntities()
        {
            var country = _context.Countries.FirstOrDefault(c => c.Name == "France");
            if (country == null)
            {
                country = new Country { Name = "France", PhonePrefixId = 75 };
                _context.Countries.Add(country);
                _context.SaveChanges();
            }
            var city = _context.Cities.FirstOrDefault(c => c.Name == "ANNECY");

            if (city == null)
            {
                city = new City { Name = "Annecy", CountryId = 75, PostCode="74000", Latitude= 45.8992348M, Longitude= 6.1288847M, CountryNavigation=country };
                _context.Cities.Add(city);
                _context.SaveChanges();
            }

            return new List<Address>
            {
                new Address { Id = 902101, StreetAddress = "Rue de l'Arc en Ciel", StreetNumber = "9", CityId = city.Id},
                new Address { Id = 902102, StreetAddress = "Rue des Fretes", StreetNumber = "3", CityId = city.Id},
            };
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
            int cityId = _context.Cities.FirstOrDefault(c => c.Name == "ANNECY").Id;

            return new AddressCreateDto
            {
                StreetAddress = "Address to insert",
                StreetNumber = "1",
                CityId = cityId
            };
        }

        protected override AddressUpdateDto GetValidUpdateDto(Address entityToUpdate)
        {
            return new AddressUpdateDto
            {
                StreetAddress = entityToUpdate.StreetAddress + " Updated",
                StreetNumber = entityToUpdate.StreetNumber,
                CityId = entityToUpdate.CityId
            };
        }

        protected override void SetIdInUpdateDto(AddressUpdateDto dto, int id)
        {
        }
    }
}