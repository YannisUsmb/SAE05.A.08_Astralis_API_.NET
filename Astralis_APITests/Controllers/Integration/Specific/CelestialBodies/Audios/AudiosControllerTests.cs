using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class AudiosControllerTests : ReadableControllerTests<AudiosController, Audio, AudioDto, AudioDto, int>
    {
        protected override AudiosController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new AudiosController(new AudioManager(context), mapper);
        }

        protected override List<Audio> GetSampleEntities()
        {
            return new List<Audio>
            {
                new Audio {Id=902101, Title = "Audio test 1", Description = "Audio Descripton test 1", FilePath="audioPath1", CelestialBodyTypeId=1},
                new Audio {Id=902102, Title = "Audio test 2", Description = "Audio Descripton test 2", FilePath="audioPath1", CelestialBodyTypeId=1}
            };
        }

        protected override int GetIdFromEntity(Audio entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}