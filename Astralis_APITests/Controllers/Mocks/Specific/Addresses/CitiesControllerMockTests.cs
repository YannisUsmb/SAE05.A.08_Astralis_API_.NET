using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class CitiesControllerMockTests : CrudControllerMockTests<CitiesController, City, CityDto, CityDto, CityCreateDto, CityCreateDto, int>
    {
        protected override CitiesController CreateController(Mock<IReadableRepository<City, int>> mockRepo, IMapper mapper)
        {
            _mockCrudRepository = mockRepo.As<ICrudRepository<City, int>>();

            var specificMock = mockRepo.As<ICityRepository>();

            return new CitiesController(specificMock.Object, mapper);
        }

        protected override City GetSampleEntity()
        {
            return new City { Id = 1, Name = "Paris", PostCode = "75000", CountryId = 10 };
        }

        protected override List<City> GetSampleEntities()
        {
            return new List<City>
            {
                new City { Id = 1, Name = "Paris", PostCode = "75000", CountryId = 10 },
                new City { Id = 2, Name = "Lyon", PostCode = "69000", CountryId = 10 }
            };
        }

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override CityCreateDto GetValidCreateDto()
        {
            return new CityCreateDto { Name = "Nice", PostCode = "06000", CountryId = 10, Latitude = 43.7m, Longitude = 7.2m };
        }

        protected override CityCreateDto GetValidUpdateDto()
        {
            return new CityCreateDto { Name = "Paris Updated", PostCode = "75001", CountryId = 10, Latitude = 48.8m, Longitude = 2.3m };
        }

        protected override void SetIdInUpdateDto(CityCreateDto dto, int id)
        {
            // No Id in the dtos
        }
    }
}