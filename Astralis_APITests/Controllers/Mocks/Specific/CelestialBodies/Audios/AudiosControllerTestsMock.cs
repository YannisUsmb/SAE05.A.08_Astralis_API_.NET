using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class AudiosControllerTestsMock : ReadableControllerMockTests<AudiosController, Audio, AudioDto, AudioDto, int>
    {
        private Mock<IAudioRepository> _mockAudioRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Audio, AudioDto>()
                   .ForMember(dest => dest.CelestialBodyTypeLabel,
                              opt => opt.MapFrom(src => src.CelestialBodyTypeNavigation.Label));
            });
            _mapper = config.CreateMapper();

            _controller = CreateController(null, _mapper);

            SetupHttpContext(1, "User");
        }

        protected override AudiosController CreateController(Mock<IReadableRepository<Audio, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockAudioRepository = new Mock<IAudioRepository>();

            _mockRepository = _mockAudioRepository.As<IReadableRepository<Audio, int>>();

            return new AudiosController(_mockAudioRepository.Object, mapper);
        }

        protected override List<Audio> GetSampleEntities() => new List<Audio>
        {
            new Audio
            {
                Id = 1,
                Title = "Sound of Saturn",
                Description = "Radio emissions from Saturn.",
                FilePath = "/audio/saturn.mp3",
                CelestialBodyTypeId = 2,
                CelestialBodyTypeNavigation = new CelestialBodyType { Id = 2, Label = "Planet" }
            },
            new Audio
            {
                Id = 2,
                Title = "Pulsar Beats",
                Description = "Regular pulses from a neutron star.",
                FilePath = "/audio/pulsar.mp3",
                CelestialBodyTypeId = 1,
                CelestialBodyTypeNavigation = new CelestialBodyType { Id = 1, Label = "Star" }
            }
        };

        protected override Audio GetSampleEntity() => new Audio
        {
            Id = 1,
            Title = "Sound of Saturn",
            Description = "Radio emissions from Saturn.",
            FilePath = "/audio/saturn.mp3",
            CelestialBodyTypeId = 2,
            CelestialBodyTypeNavigation = new CelestialBodyType { Id = 2, Label = "Planet" }
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

        [TestMethod]
        public async Task Search_WithFilters_ShouldReturnFilteredList()
        {
            // Given
            var filter = new AudioFilterDto
            {
                SearchTerm = "Saturn",
                CelestialBodyTypeIds = new List<int> { 2 }
            };

            var entities = GetSampleEntities().Where(a => a.Title.Contains("Saturn")).ToList();

            _mockAudioRepository.Setup(r => r.SearchAsync(filter.SearchTerm, filter.CelestialBodyTypeIds))
                .ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockAudioRepository.Verify(r => r.SearchAsync(filter.SearchTerm, filter.CelestialBodyTypeIds), Times.Once);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var dtos = okResult.Value as IEnumerable<AudioDto>;
            Assert.IsNotNull(dtos);
            Assert.AreEqual(1, dtos.Count());

            var firstDto = dtos.First();
            Assert.AreEqual("Sound of Saturn", firstDto.Title);
            Assert.AreEqual("Planet", firstDto.CelestialBodyTypeLabel);
        }

        [TestMethod]
        public async Task Search_WithoutFilters_ShouldReturnAll()
        {
            // Given
            var filter = new AudioFilterDto();
            var entities = GetSampleEntities();

            _mockAudioRepository.Setup(r => r.SearchAsync(null, null))
                .ReturnsAsync(entities);

            // When
            var result = await _controller.Search(filter);

            // Then
            _mockAudioRepository.Verify(r => r.SearchAsync(null, null), Times.Once);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var dtos = okResult.Value as IEnumerable<AudioDto>;
            Assert.AreEqual(2, dtos.Count());
        }
    }
}