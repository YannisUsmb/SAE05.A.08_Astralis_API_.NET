using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class ReportMotivesControllerTests : ReadableControllerTests<ReportMotivesController, ReportMotive, ReportMotiveDto, ReportMotiveDto, int>
    {
        protected override ReportMotivesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new ReportMotivesController(new ReportMotiveManager(context), mapper);
        }

        protected override List<ReportMotive> GetSampleEntities()
        {
            return new List<ReportMotive>
            {
                new ReportMotive {Id=902101, Label = "Report motive 1", Description = "ReportMotive description 1"},
                new ReportMotive {Id=902102, Label = "Report motive 2", Description = "ReportMotive description 2"}
            };
        }

        protected override int GetIdFromEntity(ReportMotive entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}