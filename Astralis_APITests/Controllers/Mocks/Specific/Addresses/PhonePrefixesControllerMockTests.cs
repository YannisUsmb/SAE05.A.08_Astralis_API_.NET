using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class PhonePrefixesControllerTestsMock : ReadableControllerMockTests<PhonePrefixesController, PhonePrefix, PhonePrefixDto, PhonePrefixDto, int>
    {
        private Mock<IPhonePrefixRepository> _mockPhonePrefixRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override PhonePrefixesController CreateController(Mock<IReadableRepository<PhonePrefix, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockPhonePrefixRepository = new Mock<IPhonePrefixRepository>();

            _mockRepository = _mockPhonePrefixRepository.As<IReadableRepository<PhonePrefix, int>>();

            return new PhonePrefixesController(_mockPhonePrefixRepository.Object, mapper);
        }

        protected override List<PhonePrefix> GetSampleEntities() => new List<PhonePrefix>
        {
            new PhonePrefix { Id = 1, Label = "+33", Example = "0612345678", RegexPattern = "^0[1-9][0-9]{8}$" },
            new PhonePrefix { Id = 2, Label = "+1", Example = "202-555-0156", RegexPattern = "^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$" }
        };

        protected override PhonePrefix GetSampleEntity() => new PhonePrefix
        {
            Id = 1,
            Label = "+33",
            Example = "0612345678",
            RegexPattern = "^0[1-9][0-9]{8}$"
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        private void SetupHttpContext(int userId, string role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            if (_controller != null)
                _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        }
    }
}