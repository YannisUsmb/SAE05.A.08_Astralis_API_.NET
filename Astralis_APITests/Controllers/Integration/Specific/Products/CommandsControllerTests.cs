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

            var existingCmd1 = _context.Commands.Find(CMD_ID_1);
            if (existingCmd1 != null) _context.Commands.Remove(existingCmd1);
            var existingCmd2 = _context.Commands.Find(CMD_ID_2);
            if (existingCmd2 != null) _context.Commands.Remove(existingCmd2);

            var existingProd = _context.Products.Find(100);
            if (existingProd != null) _context.Products.Remove(existingProd);

            if (_context.CartItems.Any()) _context.CartItems.RemoveRange(_context.CartItems);

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            if (!_context.Users.Any(u => u.Id == USER_NORMAL_ID))
                _context.Users.Add(new User { Id = USER_NORMAL_ID, LastName = "N", FirstName = "N", Email = "n@n.com", Username = "UserN", UserRoleId = 1, Password = "pwd" });

            if (!_context.Users.Any(u => u.Id == USER_ADMIN_ID))
                _context.Users.Add(new User { Id = USER_ADMIN_ID, LastName = "A", FirstName = "A", Email = "a@a.com", Username = "Admin", UserRoleId = 2, Password = "pwd" });

            if (!_context.CommandStatuses.Any(s => s.Id == STATUS_PENDING_ID))
                _context.CommandStatuses.Add(new CommandStatus { Id = STATUS_PENDING_ID, Label = "Pending" });

            if (!_context.CommandStatuses.Any(s => s.Id == STATUS_SHIPPED_ID))
                _context.CommandStatuses.Add(new CommandStatus { Id = STATUS_SHIPPED_ID, Label = "Shipped" });

            if (!_context.ProductCategories.Any(pc => pc.Id == 1))
            {
                _context.ProductCategories.Add(new ProductCategory { Id = 1, Label = "General" });
            }

            _context.SaveChanges();

            if (!_context.Products.Any(p => p.Id == 100))
            {
                _context.Products.Add(new Product
                {
                    Id = 100,
                    Label = "Test Product",
                    Price = 50,
                    Description = "Desc",
                    ProductCategoryId = 1,
                    UserId = USER_ADMIN_ID
                });
            }

            _context.SaveChanges();

            var existingItem = _context.CartItems.Find(USER_NORMAL_ID, 100);
            if (existingItem == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = USER_NORMAL_ID,
                    ProductId = 100,
                    Quantity = 2
                });
            }

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            var user = _context.Users.Find(USER_NORMAL_ID);
            var status = _context.CommandStatuses.Find(STATUS_PENDING_ID);

            var cmd1 = new Command
            {
                Id = CMD_ID_1,
                UserId = USER_NORMAL_ID,
                CommandStatusId = STATUS_PENDING_ID,
                Date = DateTime.UtcNow,
                Total = 100.00m,
                PdfName = "invoice_66001.pdf",
                PdfPath = "/files/invoices/invoice_66001.pdf",
                UserNavigation = user!,
                CommandStatusNavigation = status!
            };

            var cmd2 = new Command
            {
                Id = CMD_ID_2,
                UserId = USER_NORMAL_ID,
                CommandStatusId = STATUS_PENDING_ID,
                Date = DateTime.UtcNow.AddDays(-1),
                Total = 50.50m,
                PdfName = "invoice_66002.pdf",
                PdfPath = "/files/invoices/invoice_66002.pdf",
                UserNavigation = user!,
                CommandStatusNavigation = status!
            };

            return new List<Command> { cmd1, cmd2 };
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
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");
            var createDto = GetValidCreateDto();

            var result = await _controller.Post(createDto);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), "Le résultat devrait être un OkObjectResult");

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult?.Value, "La valeur retournée ne doit pas être nulle");

            if (okResult.Value is CommandDetailDto createdCommand)
            {
                Assert.IsTrue(createdCommand.Id > 0, "L'ID doit être généré");
                Assert.AreEqual(100.00m, createdCommand.Total, "Le total doit être correct");
            }
            else
            {
                Assert.Fail($"Type de retour inattendu : {okResult.Value.GetType().Name}");
            }
        }


        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");

            var result = await _controller.GetById(CMD_ID_1);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;

            var actualDto = okResult.Value as CommandDetailDto;
            Assert.IsNotNull(actualDto);

            Assert.AreEqual(CMD_ID_1, actualDto.Id, "L'ID de la commande est incorrect");
            Assert.AreEqual(100.00m, actualDto.Total, "Le total est incorrect");

            Assert.AreEqual("En attente", actualDto.CommandStatusLabel, "Le statut est incorrect");

            Assert.AreEqual("/files/invoices/invoice_66001.pdf", actualDto.PdfPath, "Le chemin PDF est incorrect");

            double diffSeconds = Math.Abs((actualDto.Date - DateTime.UtcNow).TotalSeconds);
            Assert.IsTrue(actualDto.Date > DateTime.MinValue, "La date n'est pas valide");
        }
    }
}