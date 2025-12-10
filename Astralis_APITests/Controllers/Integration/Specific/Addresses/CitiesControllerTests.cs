using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class CitiesControllerTests
        : CrudControllerTests<CitiesController, City, CityDto, CityDto, CityCreateDto, CityCreateDto, int>
    {
        protected override CitiesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new CitiesController(new CityManager(context), mapper);
        }

        protected override List<City> GetSampleEntities()
        {
            var country = _context.Countries.FirstOrDefault(c => c.Name == "France");
            if (country == null)
            {
                country = new Country { Name = "France", PhonePrefixId = 75 };
                _context.Countries.Add(country);
                _context.SaveChanges();
            }

            return new List<City>
            {
                new City { Id = 999997, Name = "PARIS 01", PostCode = "75001", CountryId = country.Id, Latitude = 48.8626305M, Longitude = 2.3362934M },
                new City { Id = 999998, Name = "LYON 01", PostCode = "69001", CountryId = country.Id, Latitude = 45.7699284M, Longitude = 4.8292246M }
            };
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
            int countryId = _context.Countries.First().Id;

            return new CityCreateDto
            {
                Name = "Marseille",
                PostCode = "13000",
                CountryId = countryId,
                Latitude = 43,
                Longitude = 5
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
            // CityCreateDto does not have an Id property, so nothing to set here.
        }
    }
}