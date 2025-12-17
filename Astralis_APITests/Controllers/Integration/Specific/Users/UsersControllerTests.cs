using Astralis.Shared.DTOs;
using Astralis.Shared.Enums;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class UsersControllerTests
        : CrudControllerTests<UsersController, User, UserDetailDto, UserDetailDto, UserCreateDto, UserUpdateDto, int>
    {
        private const int ROLE_ADMIN_ID = 10;
        private const int ROLE_USER_ID = 1;

        private const int USER_ADMIN_ID = 5001;
        private const int USER_NORMAL_ID = 5002;

        private int _userAdminId;
        private int _userNormalId;

        protected override UsersController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var userManager = new UserManager(context);

            var controller = new UsersController(
                userManager,
                mapper
            );

            SetupUserContext(controller, USER_ADMIN_ID, "Admin");
            return controller;
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, $"User_{userId}")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        protected override List<User> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == ROLE_ADMIN_ID))
                _context.UserRoles.Add(new UserRole { Id = ROLE_ADMIN_ID, Label = "Admin" });

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == ROLE_USER_ID))
                _context.UserRoles.Add(new UserRole { Id = ROLE_USER_ID, Label = "User" });

            _context.SaveChanges();

            var admin = new User
            {
                Id = USER_ADMIN_ID,
                UserRoleId = ROLE_ADMIN_ID,
                LastName = "Admin",
                FirstName = "Super",
                Email = "admin@astralis.com",
                Username = "AdminMaster",
                AvatarUrl = "http://avatar.com/admin.png",
                Password = "HashedPassword123",
                IsPremium = true
            };

            var normalUser = new User
            {
                Id = USER_NORMAL_ID,
                UserRoleId = ROLE_USER_ID,
                LastName = "Doe",
                FirstName = "John",
                Email = "john.doe@test.com",
                Username = "JohnDoe",
                AvatarUrl = "http://avatar.com/john.png",
                Password = "HashedPassword456",
                IsPremium = false
            };

            _userAdminId = USER_ADMIN_ID;
            _userNormalId = USER_NORMAL_ID;

            return new List<User> { admin, normalUser };
        }
        protected override int GetIdFromEntity(User entity) => entity.Id;
        protected override int GetIdFromDto(UserDetailDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override UserCreateDto GetValidCreateDto()
        {
            return new UserCreateDto
            {
                LastName = "New",
                FirstName = "User",
                Email = "new.user@test.com",
                Username = "NewUser123",
                UserAvatarUrl = "http://test.com/img.png",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Gender = (GenderType)1,
                MultiFactorAuthentification = false
            };
        }

        protected override UserUpdateDto GetValidUpdateDto(User entityToUpdate)
        {
            return new UserUpdateDto
            {
                LastName = "UpdatedName",
                FirstName = "UpdatedFirst",
                Email = "updated@test.com",
                Username = "UpdatedUser",
                UserAvatarUrl = "http://updated.com/img.png",
                Gender = (GenderType)1,
                MultiFactorAuthentification = true
            };
        }

        protected override void SetIdInUpdateDto(UserUpdateDto dto, int id) { }


        [TestMethod]
        public new async Task Put_NonExistingId_ShouldReturn404()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");
            var updateDto = GetValidUpdateDto(new User());

            var result = await _controller.Put(GetNonExistingId(), updateDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public new async Task GetAll_ShouldReturnOk()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task ChangePassword_Self_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var pwdDto = new ChangePasswordDto
            {
                CurrentPassword = "CurrentPassword123!",
                NewPassword = "NewPassword123!"
            };

            var result = await _controller.ChangePassword(USER_NORMAL_ID, pwdDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var userInDb = await _context.Users.FindAsync(USER_NORMAL_ID);
            Assert.AreEqual("NewPassword123!", userInDb.Password);
        }

        [TestMethod]
        public async Task ChangePassword_Other_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var pwdDto = new ChangePasswordDto
            {
                CurrentPassword = "CurrentPassword256!",
                NewPassword = "CurrentPassword256Changed!"
            };

            var result = await _controller.ChangePassword(USER_ADMIN_ID, pwdDto);
            
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");
            var updateDto = GetValidUpdateDto(new User());

            var result = await _controller.Put(USER_ADMIN_ID, updateDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}