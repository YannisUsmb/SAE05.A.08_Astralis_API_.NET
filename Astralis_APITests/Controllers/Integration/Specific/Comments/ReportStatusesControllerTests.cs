using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class ReportStatusesControllerTests : ReadableControllerTests<ReportStatusesController, ReportStatus, ReportStatusDto, ReportStatusDto, int>
    {
        protected override ReportStatusesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new ReportStatusesController(new ReportStatusManager(context), mapper);
        }

        protected override List<ReportStatus> GetSampleEntities()
        {
            return new List<ReportStatus>
            {
                new ReportStatus {Id=902101, Label = "Report motive 1"},
                new ReportStatus {Id=902102, Label = "Report motive 2"}
            };
        }

        protected override int GetIdFromEntity(ReportStatus entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}