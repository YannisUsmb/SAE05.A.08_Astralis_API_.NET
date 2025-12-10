using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class CommandStatusesControllerTests : ReadableControllerTests<CommandStatusesController, CommandStatus, CommandStatusDto, CommandStatusDto, int>
    {
        protected override CommandStatusesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new CommandStatusesController(new CommandStatusManager(context), mapper);
        }

        protected override List<CommandStatus> GetSampleEntities()
        {
            return new List<CommandStatus>
            {
                new CommandStatus {Id=902101, Label = "Command Status 1"},
                new CommandStatus {Id=902102, Label = "Command Status 2"}
            };
        }

        protected override int GetIdFromEntity(CommandStatus entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}