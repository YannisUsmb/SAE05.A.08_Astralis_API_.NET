using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class PlanetTypesControllerMockTests
            : ReadableControllerMockTests<PlanetTypesController, PlanetType, PlanetTypeDto, PlanetTypeDto, int>
    {
        protected override PlanetTypesController CreateController(Mock<IReadableRepository<PlanetType, int>> mockRepo, IMapper mapper)
        {
            var specificMock = mockRepo.As<IPlanetTypeRepository>();

            return new PlanetTypesController(specificMock.Object, mapper);
        }

        protected override List<PlanetType> GetSampleEntities()
        {
            return new List<PlanetType>
            {
                new PlanetType { Id = 1, Label = "Ice Giant", Description = "Giant planet composed mainly of elements heavier than hydrogen." },
                new PlanetType { Id = 2, Label = "Dwarf Planet", Description = "Planetary-mass object that does not dominate its region of space." }
            };
        }

        protected override PlanetType GetSampleEntity()
        {
            return new PlanetType
            {
                Id = 1,
                Label = "Ice Giant",
                Description = "Giant planet composed mainly of elements heavier than hydrogen."
            };
        }

        protected override int GetExistingId()
        {
            return 1;
        }

        protected override int GetNonExistingId()
        {
            return 99;
        }
    }
}