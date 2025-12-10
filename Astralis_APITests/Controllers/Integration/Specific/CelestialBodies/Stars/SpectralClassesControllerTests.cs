using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class SpectralClassesControllerTests : ReadableControllerTests<SpectralClassesController, SpectralClass, SpectralClassDto, SpectralClassDto, int>
    {
        protected override SpectralClassesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new SpectralClassesController(new SpectralClassManager(context), mapper);
        }

        protected override List<SpectralClass> GetSampleEntities()
        {
            return new List<SpectralClass>
            {
                new SpectralClass {Id=902101, Label = "SC1", Description = "SpectralClass description 1"},
                new SpectralClass {Id=902102, Label = "SC2", Description = "SpectralClass description 2"}
            };
        }

        protected override int GetIdFromEntity(SpectralClass entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}