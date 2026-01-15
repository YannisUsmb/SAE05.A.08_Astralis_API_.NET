using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class UsersControllerTestsMock : CrudControllerMockTests<UsersController, User, UserDetailDto, UserDetailDto, UserCreateDto, UserUpdateDto, int>
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ICountryRepository> _mockCountryRepository;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private Mock<IEmailService> _mockEmailService;
        private Mock<IUploadService> _mockUploadService;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            SetupHttpContext(1, "Admin");
        }

        protected override UsersController CreateController(Mock<IReadableRepository<User, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockUserRepository = new Mock<IUserRepository>();

            _mockCrudRepository = _mockUserRepository.As<ICrudRepository<User, int>>();
            _mockRepository = _mockUserRepository.As<IReadableRepository<User, int>>();

            _mockCountryRepository = new Mock<ICountryRepository>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockEmailService = new Mock<IEmailService>();
            _mockUploadService = new Mock<IUploadService>();

            var controller = new UsersController(
                _mockUserRepository.Object,
                _mockCountryRepository.Object,
                mapper,
                _mockWebHostEnvironment.Object,
                _mockEmailService.Object,
                _mockUploadService.Object
            );

            return controller;
        }

        protected override void SetIdInUpdateDto(UserUpdateDto dto, int id) { }
        protected override List<User> GetSampleEntities() => new List<User>
        {
            new User { Id = 1, Username = "Alice", Email = "alice@test.com", UserRoleId = 1 },
            new User { Id = 2, Username = "Bob", Email = "bob@test.com", UserRoleId = 1 }
        };

        protected override User GetSampleEntity() => new User
        {
            Id = 1,
            Username = "Alice",
            Email = "alice@test.com",
            UserRoleId = 1,
            Password = BCrypt.Net.BCrypt.HashPassword("OldPass"),
            AvatarUrl = "old_avatar.jpg",
            PhonePrefixId = 33
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override UserCreateDto GetValidCreateDto() => new UserCreateDto
        {
            Username = "NewUser",
            Email = "new@test.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            CountryId = 1
        };

        protected override UserUpdateDto GetValidUpdateDto() => new UserUpdateDto
        {
            Username = "UpdatedUser",
            Email = "updated@test.com",
            FirstName = "John",
            LastName = "Doe",
            AvatarUrl = "new_avatar.jpg"
        };

        private void SetupHttpContext(int userId, string role = "User")
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Role, role) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            if (_controller != null)
                _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            var createDto = GetValidCreateDto();
            int countryId = createDto.CountryId.GetValueOrDefault(1);

            _mockCountryRepository.Setup(r => r.GetByIdAsync(countryId)).ReturnsAsync(new Country { Id = countryId, PhonePrefixId = 33 });
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var entity = GetSampleEntity();
            SetupHttpContext(id, "Admin");

            _mockUserRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mockUserRepository.Setup(r => r.AnonymizeUserAsync(id)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Delete(id);

            // Then
            _mockUserRepository.Verify(r => r.DeleteAsync(It.IsAny<User>()), Times.Never);
            _mockUserRepository.Verify(r => r.AnonymizeUserAsync(id), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            int id = GetExistingId();
            var updateDto = GetValidUpdateDto();
            var existingEntity = GetSampleEntity();
            SetupHttpContext(id, "User");

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingEntity);
            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<User>())).Returns(Task.CompletedTask);

            // When
            IActionResult actionResult = await _controller.Put(id, updateDto);

            // Then
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(id), Times.AtLeastOnce);
            _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<User>()), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Put_NonExistingObject_ShouldReturnNotFound()
        {
            // Given
            int id = GetNonExistingId();
            SetupHttpContext(1, "Admin");

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((User)null);

            // When
            IActionResult actionResult = await _controller.Put(id, GetValidUpdateDto());

            // Then
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(id), Times.AtLeastOnce);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CheckAvailability_WhenEmailExists_ShouldReturnTrue()
        {
            // Given
            string email = "taken@test.com";
            _mockUserRepository.Setup(r => r.ExistsByEmailAsync(email)).ReturnsAsync(true);

            // When
            var result = await _controller.CheckAvailability(email, null, null, null) as OkObjectResult;

            // Then
            Assert.IsNotNull(result);
            var props = result.Value.GetType().GetProperties();
            var isTaken = (bool)props.First(p => p.Name == "isTaken").GetValue(result.Value);
            Assert.IsTrue(isTaken);
        }

        [TestMethod]
        public async Task Post_WithCountry_ShouldFetchPrefixAndSendEmail()
        {
            // Given
            SetupHttpContext(1, "Admin");
            var createDto = GetValidCreateDto();
            createDto.CountryId = 5;
            var country = new Country { Id = 5, PhonePrefixId = 33 };

            _mockCountryRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(country);
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockCountryRepository.Verify(r => r.GetByIdAsync(5), Times.Once);
            _mockUserRepository.Verify(r => r.AddAsync(It.Is<User>(u => u.PhonePrefixId == 33)), Times.Once);
            _mockEmailService.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Put_WithNewAvatar_ShouldDeleteOldFile()
        {
            // Given
            int id = 1;
            SetupHttpContext(id, "User");
            var existingUser = GetSampleEntity();
            var updateDto = GetValidUpdateDto();
            updateDto.AvatarUrl = "new_avatar.jpg";

            _mockUserRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<User>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Put(id, updateDto);

            // Then
            _mockUploadService.Verify(u => u.DeleteFileAsync("old_avatar.jpg", "avatars"), Times.Once);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task ChangePassword_WithCorrectOldPassword_ShouldUpdate()
        {
            // Given
            int id = 1;
            SetupHttpContext(id, "User");
            var user = GetSampleEntity();
            var pwdDto = new ChangePasswordDto { CurrentPassword = "OldPass", NewPassword = "NewPass123" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateAsync(user, user)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.ChangePassword(id, pwdDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task ChangePassword_WithWrongOldPassword_ShouldFail()
        {
            // Given
            int id = 1;
            SetupHttpContext(id, "User");
            var user = GetSampleEntity();
            var pwdDto = new ChangePasswordDto { CurrentPassword = "WRONG_PASSWORD", NewPassword = "NewPass123" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

            // When
            var result = await _controller.ChangePassword(id, pwdDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public async Task AnonymizeData_ShouldCallRepoMethod()
        {
            // Given
            int id = 1;
            SetupHttpContext(id, "User");
            var user = GetSampleEntity();

            _mockUserRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.AnonymizeUserAsync(id)).Returns(Task.CompletedTask);

            // When
            var result = await _controller.AnonymizeData(id);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockUserRepository.Verify(r => r.AnonymizeUserAsync(id), Times.Once);
        }
    }
}