using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class DetectionMethodsControllerTests : ReadableControllerTests<DetectionMethodsController, DetectionMethod, DetectionMethodDto, DetectionMethodDto, int>
    {
        protected override DetectionMethodsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new DetectionMethodsController(new DetectionMethodManager(context), mapper);
        }

        protected override List<DetectionMethod> GetSampleEntities()
        {
            return new List<DetectionMethod>
            {
                new DetectionMethod {Id=902101, Label = "DetectionMethod 1", Description = "DetectionMethod description 1"},
                new DetectionMethod {Id=902102, Label = "DetectionMethod 2", Description = "DetectionMethod description 2"}
            };
        }

        protected override int GetIdFromEntity(DetectionMethod entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}