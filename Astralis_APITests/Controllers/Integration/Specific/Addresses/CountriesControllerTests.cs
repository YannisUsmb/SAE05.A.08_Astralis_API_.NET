using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class CountriesControllerTests : ReadableControllerTests<CountriesController, Country, CountryDto, CountryDto, int>
    {
        protected override CountriesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new CountriesController(new CountryManager(context), mapper);
        }

        protected override List<Country> GetSampleEntities()
        {
            return new List<Country>
        {
            new Country {Id=21102200, Name = "TestType1", PhonePrefixId = 1 },
            new Country {Id=21102201, Name = "TestType2", PhonePrefixId = 1 }
        };
        }

        protected override int GetIdFromEntity(Country entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}