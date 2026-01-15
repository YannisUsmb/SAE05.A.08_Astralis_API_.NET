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
    public class DetectionMethodsControllerTestsMock : ReadableControllerMockTests<DetectionMethodsController, DetectionMethod, DetectionMethodDto, DetectionMethodDto, int>
    {
        private Mock<IDetectionMethodRepository> _mockDetectionMethodRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override DetectionMethodsController CreateController(Mock<IReadableRepository<DetectionMethod, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            // Given
            _mockDetectionMethodRepository = new Mock<IDetectionMethodRepository>();

            // When
            _mockRepository = _mockDetectionMethodRepository.As<IReadableRepository<DetectionMethod, int>>();

            // Then
            return new DetectionMethodsController(_mockDetectionMethodRepository.Object, mapper);
        }


        protected override List<DetectionMethod> GetSampleEntities() => new List<DetectionMethod>
        {
            new DetectionMethod
            {
                Id = 1,
                Label = "Radial Velocity",
                Description = "Detects Doppler shifts in the star's spectrum."
            },
            new DetectionMethod
            {
                Id = 2,
                Label = "Transit Photometry",
                Description = "Detects dips in star brightness."
            }
        };

        protected override DetectionMethod GetSampleEntity() => new DetectionMethod
        {
            Id = 1,
            Label = "Radial Velocity",
            Description = "Detects Doppler shifts in the star's spectrum."
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
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
                };
        }
    }
}