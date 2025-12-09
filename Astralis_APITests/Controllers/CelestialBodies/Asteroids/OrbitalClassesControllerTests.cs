using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class OrbitalClasssControllerTests : ReadableControllerTests<OrbitalClassesController, OrbitalClass, OrbitalClassDto, OrbitalClassDto, int>
    {
        protected override OrbitalClassesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new OrbitalClassesController(new OrbitalClassManager(context), mapper);
        }

        protected override List<OrbitalClass> GetSampleEntities()
        {
            return new List<OrbitalClass>
            {
                new OrbitalClass {Id=902101, Label="AT1" ,Description = "Article type 1" },
                new OrbitalClass {Id=902102, Label="AT2" ,Description = "Article type 2" }
            };
        }

        protected override int GetIdFromEntity(OrbitalClass entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}