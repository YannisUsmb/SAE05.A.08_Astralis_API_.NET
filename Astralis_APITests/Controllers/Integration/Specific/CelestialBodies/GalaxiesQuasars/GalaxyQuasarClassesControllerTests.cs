using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class GalaxyQuasarClassesControllerTests : ReadableControllerTests<GalaxyQuasarClassesController, GalaxyQuasarClass, GalaxyQuasarClassDto, GalaxyQuasarClassDto, int>
    {
        protected override GalaxyQuasarClassesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new GalaxyQuasarClassesController(new GalaxyQuasarClassManager(context), mapper);
        }

        protected override List<GalaxyQuasarClass> GetSampleEntities()
        {
            return new List<GalaxyQuasarClass>
            {
                new GalaxyQuasarClass {Id=902101, Label = "GalaxyQuasarClass 1", Description = "GalaxyQuasarClass description 1"},
                new GalaxyQuasarClass {Id=902102, Label = "GalaxyQuasarClass 2", Description = "GalaxyQuasarClass description 2"}
            };
        }

        protected override int GetIdFromEntity(GalaxyQuasarClass entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}