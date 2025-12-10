using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class CelestialBodyTypesControllerTests : ReadableControllerTests<CelestialBodyTypesController, CelestialBodyType, CelestialBodyTypeDto, CelestialBodyTypeDto, int>
    {
        protected override CelestialBodyTypesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new CelestialBodyTypesController(new CelestialBodyTypeManager(context), mapper);
        }

        protected override List<CelestialBodyType> GetSampleEntities()
        {
            return new List<CelestialBodyType>
            {
                new CelestialBodyType {Id=902101, Label = "Celestial Body Type 1", Description="Celestial Body Type Description 1"},
                new CelestialBodyType {Id=902102, Label = "Celestial Body Type 2", Description="Celestial Body Type Description 2"}
            };
        }

        protected override int GetIdFromEntity(CelestialBodyType entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}