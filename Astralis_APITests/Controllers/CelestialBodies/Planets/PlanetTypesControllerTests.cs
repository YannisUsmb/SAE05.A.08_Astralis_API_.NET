using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class PlanetTypesControllerTests : ReadableControllerTests<PlanetTypesController, PlanetType, PlanetTypeDto, PlanetTypeDto, int>
    {
        protected override PlanetTypesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new PlanetTypesController(new PlanetTypeManager(context), mapper);
        }

        protected override List<PlanetType> GetSampleEntities()
        {
            return new List<PlanetType>
        {
            new PlanetType { Label = "TestType1", Description = "Desc1" },
            new PlanetType { Label = "TestType2", Description = "Desc2" }
        };
        }

        protected override int GetIdFromEntity(PlanetType entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}