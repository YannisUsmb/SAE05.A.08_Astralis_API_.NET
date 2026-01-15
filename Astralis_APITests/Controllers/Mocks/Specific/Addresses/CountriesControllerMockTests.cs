using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class CountriesControllerTestsMock : ReadableControllerMockTests<CountriesController, Country, CountryDto, CountryDto, int>
    {
        private Mock<ICountryRepository> _mockCountryRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "User");
        }

        protected override CountriesController CreateController(Mock<IReadableRepository<Country, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockCountryRepository = new Mock<ICountryRepository>();

            _mockRepository = _mockCountryRepository.As<IReadableRepository<Country, int>>();

            return new CountriesController(_mockCountryRepository.Object, mapper);
        }

        protected override List<Country> GetSampleEntities() => new List<Country>
        {
            new Country
            {
                Id = 1,
                Name = "France",
                IsoCode = "FR",
                PhonePrefixId = 1
            },
            new Country
            {
                Id = 2,
                Name = "United States",
                IsoCode = "US",
                PhonePrefixId = 2
            }
        };

        protected override Country GetSampleEntity() => new Country
        {
            Id = 1,
            Name = "France",
            IsoCode = "FR",
            PhonePrefixId = 1
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