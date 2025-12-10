using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class PhonePrefixesControllerTests : ReadableControllerTests<PhonePrefixesController, PhonePrefix, PhonePrefixDto, PhonePrefixDto, int>
    {
        protected override PhonePrefixesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new PhonePrefixesController(new PhonePrefixManager(context), mapper);
        }

        protected override List<PhonePrefix> GetSampleEntities()
        {
            return new List<PhonePrefix>
        {
            new PhonePrefix {Id=21102200, Label = "+22221"},
            new PhonePrefix {Id=21102201, Label = "+22222"}
        };
        }

        protected override int GetIdFromEntity(PhonePrefix entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}