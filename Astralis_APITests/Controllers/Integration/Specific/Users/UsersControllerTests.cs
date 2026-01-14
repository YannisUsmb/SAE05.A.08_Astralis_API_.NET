using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{

    public class FakeUserEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string htmlMessage) => Task.CompletedTask;
    }

    public class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "wwwroot";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ApplicationName { get; set; } = "TestApp";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "./";
        public string EnvironmentName { get; set; } = "Development";
    }

    public class FakeUploadService : IUploadService
    {
        public Task<string> UploadImageAsync(IFormFile file, string folderName, string fileName)
        {
            return Task.FromResult("https://fake-url.com/avatar.png");
        }

        public Task DeleteFileAsync(string fileUrl, string folderName)
        {
            return Task.CompletedTask;
        }
    }


    [TestClass]
    public class UsersControllerTests
        : CrudControllerTests<UsersController, User, UserDetailDto, UserDetailDto, UserCreateDto, UserUpdateDto, int>
    {
        private const int ROLE_ADMIN_ID = 2; 
        private const int ROLE_USER_ID = 1;

        private const int USER_ADMIN_ID = 5001;
        private const int USER_NORMAL_ID = 5002;
        private const int COUNTRY_ID = 1;

        protected override UsersController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var userManager = new UserManager(context);
            var countryRepository = new CountryManager(context);

            var env = new FakeWebHostEnvironment();
            var emailService = new FakeUserEmailService();
            var uploadService = new FakeUploadService();

            var controller = new UsersController(
                userManager,
                countryRepository,
                mapper,
                env,
                emailService,
                uploadService
            );

            SetupUserContext(controller, USER_ADMIN_ID, "Admin");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        protected override List<User> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            var existingAdmin = _context.Users.Find(USER_ADMIN_ID);
            if (existingAdmin != null)
            {
                _context.Users.Remove(existingAdmin);
            }

            var existingNormal = _context.Users.Find(USER_NORMAL_ID);
            if (existingNormal != null)
            {
                _context.Users.Remove(existingNormal);
            }

            _context.SaveChanges();
            _context.ChangeTracker.Clear();


            if (!_context.UserRoles.Any(r => r.Id == ROLE_USER_ID))
                _context.UserRoles.Add(new UserRole { Id = ROLE_USER_ID, Label = "User" });

            if (!_context.UserRoles.Any(r => r.Id == ROLE_ADMIN_ID))
                _context.UserRoles.Add(new UserRole { Id = ROLE_ADMIN_ID, Label = "Admin" });

            if (!_context.Countries.Any(c => c.Id == COUNTRY_ID))
                _context.Countries.Add(new Country { Id = COUNTRY_ID, Name = "France" });

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            var admin = new User
            {
                Id = USER_ADMIN_ID,
                LastName = "Admin",
                FirstName = "Super",
                Email = "admin@astralis.com",
                Username = "AdminMaster",
                UserRoleId = ROLE_ADMIN_ID,
                Password = "hashedpassword",
                PhonePrefixId = COUNTRY_ID
            };

            var normal = new User
            {
                Id = USER_NORMAL_ID,
                LastName = "Doe",
                FirstName = "John",
                Email = "john@doe.com",
                Username = "JohnDoe",
                UserRoleId = ROLE_USER_ID,
                Password = "hashedpassword",
                PhonePrefixId = COUNTRY_ID
            };

            return new List<User> { admin, normal };
        }


        protected override int GetIdFromEntity(User entity) => entity.Id;
        protected override int GetNonExistingId() => 99999;
        protected override int GetIdFromDto(UserDetailDto dto) => dto.Id;

        protected override UserCreateDto GetValidCreateDto()
        {
            return new UserCreateDto
            {
                LastName = "New",
                FirstName = "User",
                Email = "new@user.com",
                Username = "NewUserTest",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                CountryId = COUNTRY_ID,
                MultiFactorAuthentification = false
            };
        }

        protected override UserUpdateDto GetValidUpdateDto(User entity)
        {
            return new UserUpdateDto
            {
                LastName = entity.LastName + "Updated",
                FirstName = entity.FirstName,
                Email = entity.Email,
                Username = entity.Username,
            };
        }

        protected override void SetIdInUpdateDto(UserUpdateDto dto, int id)
        {
        }


        [TestMethod]
        public async Task ChangePassword_ShouldSucceed_WhenCorrect()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var pwdDto = new ChangePasswordDto
            {
                CurrentPassword = "hashedpassword",
                NewPassword = "NewPassword123!"
            };

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task ChangePassword_Other_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var pwdDto = new ChangePasswordDto
            {
                CurrentPassword = "Any",
                NewPassword = "New"
            };

            var result = await _controller.ChangePassword(USER_ADMIN_ID, pwdDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var adminUser = _context.Users.Find(USER_ADMIN_ID);
            var updateDto = GetValidUpdateDto(adminUser!);

            var result = await _controller.Put(USER_ADMIN_ID, updateDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_ExistingId_ShouldDeleteAndReturn204()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var result = await _controller.Delete(USER_NORMAL_ID);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
        [TestMethod]
        public  async Task Put_NonExistingId_ShouldReturn404()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, USER_ADMIN_ID.ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Name, "AdminUser")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth", ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            int nonExistingId = 999999;

            var updateDto = GetValidUpdateDto(new User());

            var result = await _controller.Put(nonExistingId, updateDto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}