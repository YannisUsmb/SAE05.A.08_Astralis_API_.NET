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
    public class SpectralClassesControllerTestsMock : ReadableControllerMockTests<SpectralClassesController, SpectralClass, SpectralClassDto, SpectralClassDto, int>
    {
        private Mock<ISpectralClassRepository> _mockSpectralClassRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override SpectralClassesController CreateController(Mock<IReadableRepository<SpectralClass, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            // Given
            _mockSpectralClassRepository = new Mock<ISpectralClassRepository>();

            // When
            _mockRepository = _mockSpectralClassRepository.As<IReadableRepository<SpectralClass, int>>();

            // Then
            return new SpectralClassesController(_mockSpectralClassRepository.Object, mapper);
        }


        protected override List<SpectralClass> GetSampleEntities() => new List<SpectralClass>
        {
            new SpectralClass
            {
                Id = 1,
                Label = "O",
                Description = "Blue, very hot stars (> 30,000 K)"
            },
            new SpectralClass
            {
                Id = 2,
                Label = "G",
                Description = "Yellow stars like the Sun (5,200 - 6,000 K)"
            }
        };

        protected override SpectralClass GetSampleEntity() => new SpectralClass
        {
            Id = 1,
            Label = "O",
            Description = "Blue, very hot stars (> 30,000 K)"
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