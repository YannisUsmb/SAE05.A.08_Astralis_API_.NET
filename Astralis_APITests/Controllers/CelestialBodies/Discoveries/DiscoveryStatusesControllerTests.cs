using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class DiscoveryStatuss : ReadableControllerTests<DiscoveryStatusesController, DiscoveryStatus, DiscoveryStatusDto, DiscoveryStatusDto, int>
    {
        protected override DiscoveryStatusesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new DiscoveryStatusesController(new DiscoveryStatusManager(context), mapper);
        }

        protected override List<DiscoveryStatus> GetSampleEntities()
        {
            return new List<DiscoveryStatus>
            {
                new DiscoveryStatus {Id=902101, Label = "DiscoveryStatus 1"},
                new DiscoveryStatus {Id=902102, Label = "DiscoveryStatus 2"}
            };
        }

        protected override int GetIdFromEntity(DiscoveryStatus entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}