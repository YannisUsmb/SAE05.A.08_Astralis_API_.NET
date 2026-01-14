using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    public class FakeCommandEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string htmlMessage) => Task.CompletedTask;
    }

    [TestClass]
    public class CommandsControllerTests
        : CrudControllerTests<CommandsController, Command, CommandListDto, CommandDetailDto, CommandCheckoutDto, CommandUpdateDto, int>
    {
        private const int USER_ADMIN_ID = 5001;
        private const int USER_NORMAL_ID = 5002;

        private const int STATUS_PENDING_ID = 1;
        private const int STATUS_SHIPPED_ID = 2;

        private const int CMD_ID_1 = 66001;
        private const int CMD_ID_2 = 66002;

        protected override CommandsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var commandManager = new CommandManager(context);
            var cartItemManager = new CartItemManager(context);
            var orderDetailManager = new OrderDetailManager(context);
            var userManager = new UserManager(context);

            var emailService = new FakeCommandEmailService();

            var myConfig = new Dictionary<string, string>
            {
                {"Stripe:SecretKey", "sk_test_fake_key_12345"},
                {"ClientUrl", "http://localhost:3000"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfig)
                .Build();

            var controller = new CommandsController(
                commandManager,
                cartItemManager,
                orderDetailManager,
                mapper,
                configuration,
                userManager,
                emailService
            );

            SetupUserContext(controller, USER_ADMIN_ID, "Admin");

            return controller;
        }

        private void SetupUserContext(CommandsController controller, int userId, string role)
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

        protected override List<Command> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            if (!_context.Users.Any(u => u.Id == USER_NORMAL_ID))
                _context.Users.Add(new User { Id = USER_NORMAL_ID, LastName = "N", FirstName = "N", Email = "n@n.com", Username = "UserN", UserRoleId = 1, Password = "pwd" });

            if (!_context.Users.Any(u => u.Id == USER_ADMIN_ID))
                _context.Users.Add(new User { Id = USER_ADMIN_ID, LastName = "A", FirstName = "A", Email = "a@a.com", Username = "Admin", UserRoleId = 2, Password = "pwd" });

            if (!_context.CommandStatuses.Any())
            {
                _context.CommandStatuses.Add(new CommandStatus { Id = STATUS_PENDING_ID, Label = "Pending" });
                _context.CommandStatuses.Add(new CommandStatus { Id = STATUS_SHIPPED_ID, Label = "Shipped" });
            }

            return new List<Command>
            {
                new Command
                {
                    Id = CMD_ID_1,
                    UserId = USER_NORMAL_ID,
                    CommandStatusId = STATUS_PENDING_ID,
                    Date = DateTime.UtcNow,
                    Total = 100.00m
                },
                new Command
                {
                    Id = CMD_ID_2,
                    UserId = USER_NORMAL_ID,
                    CommandStatusId = STATUS_SHIPPED_ID,
                    Date = DateTime.UtcNow.AddDays(-1),
                    Total = 50.00m
                }
            };
        }


        protected override int GetIdFromEntity(Command entity) => entity.Id;
        protected override int GetNonExistingId() => 99999;
        protected override int GetIdFromDto(CommandDetailDto dto) => dto.Id;

        protected override CommandCheckoutDto GetValidCreateDto()
        {
            return new CommandCheckoutDto
            {
            };
        }

        protected override CommandUpdateDto GetValidUpdateDto(Command entity)
        {
            return new CommandUpdateDto
            {
                CommandStatusId = STATUS_SHIPPED_ID
            };
        }

        protected override void SetIdInUpdateDto(CommandUpdateDto dto, int id)
        {
        }


        [TestMethod]
        public async Task Put_UpdateStatus_ShouldSuccess()
        {
            // Given
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");

            var updateDto = new CommandUpdateDto
            {
                CommandStatusId = STATUS_SHIPPED_ID
            };

            // When
            var result = await _controller.Put(CMD_ID_1, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var entity = await _context.Commands.FindAsync(CMD_ID_1);
            Assert.AreEqual(STATUS_SHIPPED_ID, entity!.CommandStatusId);
        }

        [TestMethod]
        public async Task Put_NormalUser_ShouldReturnForbidden()
        {
            // Given
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var updateDto = new CommandUpdateDto
            {
                CommandStatusId = STATUS_SHIPPED_ID
            };

            // When
            var result = await _controller.Put(CMD_ID_1, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}