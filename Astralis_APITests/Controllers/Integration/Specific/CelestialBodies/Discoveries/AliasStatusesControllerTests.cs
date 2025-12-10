using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class AliasStatusesControllerTests : ReadableControllerTests<AliasStatusesController, AliasStatus, AliasStatusDto, AliasStatusDto, int>
    {
        protected override AliasStatusesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new AliasStatusesController(new AliasStatusManager(context), mapper);
        }

        protected override List<AliasStatus> GetSampleEntities()
        {
            return new List<AliasStatus>
            {
                new AliasStatus {Id=902101, Label = "AliasStatus 1"},
                new AliasStatus {Id=902102, Label = "AliasStatus 2"}
            };
        }

        protected override int GetIdFromEntity(AliasStatus entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}