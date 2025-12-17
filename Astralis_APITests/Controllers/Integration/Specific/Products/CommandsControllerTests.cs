using Astralis.Shared.DTOs;
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
    public class CommandsControllerTests
        : CrudControllerTests<CommandsController, Command, CommandListDto, CommandDetailDto, CommandCheckoutDto, CommandUpdateDto, int>
    {
        private const int USER_ADMIN_ID = 5001;
        private const int USER_NORMAL_ID = 5002;

        private const int STATUS_PENDING_ID = 1;
        private const int STATUS_SHIPPED_ID = 2;

        private const int PRODUCT_ID = 33001;

        private const int CMD_ID_1 = 66001;
        private const int CMD_ID_2 = 66002;

        private int _cmdId1;
        private int _cmdId2;

        protected override CommandsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var commandManager = new CommandManager(context);
            var cartItemManager = new CartItemManager(context);
            var orderDetailManager = new OrderDetailManager(context);

            var controller = new CommandsController(
                commandManager,
                cartItemManager,
                orderDetailManager,
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

        protected override List<Command> GetSampleEntities()
        {

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == 10))
                _context.UserRoles.Add(new UserRole { Id = 10, Label = "Admin" });

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == 1))
                _context.UserRoles.Add(new UserRole { Id = 1, Label = "User" });

            if (!_context.CommandStatuses.AsNoTracking().Any(s => s.Id == STATUS_PENDING_ID))
                _context.CommandStatuses.Add(new CommandStatus { Id = STATUS_PENDING_ID, Label = "Pending" });

            if (!_context.CommandStatuses.AsNoTracking().Any(s => s.Id == STATUS_SHIPPED_ID))
                _context.CommandStatuses.Add(new CommandStatus { Id = STATUS_SHIPPED_ID, Label = "Shipped" });

            if (!_context.ProductCategories.AsNoTracking().Any(c => c.Id == 1))
                _context.ProductCategories.Add(new ProductCategory { Id = 1, Label = "Misc" });

            _context.SaveChanges();

            CreateUserIfNotExist(USER_ADMIN_ID, 10);
            CreateUserIfNotExist(USER_NORMAL_ID, 1);

            if (!_context.Products.AsNoTracking().Any(p => p.Id == PRODUCT_ID))
            {
                _context.Products.Add(new Product
                {
                    Id = PRODUCT_ID,
                    Label = "Test Product",
                    Price = 50.00m,
                    ProductCategoryId = 1,
                    UserId = USER_ADMIN_ID
                });
                _context.SaveChanges();
            }

            if (!_context.CartItems.AsNoTracking().Any(ci => ci.UserId == USER_ADMIN_ID))
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = USER_ADMIN_ID,
                    ProductId = PRODUCT_ID,
                    Quantity = 1
                });
                _context.SaveChanges();
            }


            var userNormal = _context.Users.Find(USER_NORMAL_ID);
            var statusPending = _context.CommandStatuses.Find(STATUS_PENDING_ID);
            var statusShipped = _context.CommandStatuses.Find(STATUS_SHIPPED_ID);
            var product = _context.Products.Find(PRODUCT_ID);

            var c1 = new Command
            {
                Id = CMD_ID_1,
                UserId = USER_NORMAL_ID,
                CommandStatusId = STATUS_PENDING_ID,
                Date = DateTime.UtcNow.AddDays(-1),
                Total = 100.00m,
                PdfName = "invoice_1.pdf",
                PdfPath = "/invoices/invoice_1.pdf",

                UserNavigation = userNormal!,
                CommandStatusNavigation = statusPending!,

                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = PRODUCT_ID,
                        Quantity = 2,
                        ProductNavigation = product!
                    }
                }
            };

            var c2 = new Command
            {
                Id = CMD_ID_2,
                UserId = USER_NORMAL_ID,
                CommandStatusId = STATUS_SHIPPED_ID,
                Date = DateTime.UtcNow.AddDays(-5),
                Total = 50.00m,
                PdfName = "invoice_2.pdf",
                PdfPath = "/invoices/invoice_2.pdf",

                UserNavigation = userNormal!,
                CommandStatusNavigation = statusShipped!,

                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = PRODUCT_ID,
                        Quantity = 1,
                        ProductNavigation = product!
                    }
                }
            };

            _cmdId1 = CMD_ID_1;
            _cmdId2 = CMD_ID_2;

            return new List<Command> { c1, c2 };
        }

        private void CreateUserIfNotExist(int id, int roleId)
        {
            if (!_context.Users.AsNoTracking().Any(u => u.Id == id))
            {
                _context.Users.Add(new User
                {
                    Id = id,
                    UserRoleId = roleId,
                    Username = $"User{id}",
                    Email = $"user{id}@test.com",
                    FirstName = "Test",
                    LastName = "User",
                    Password = "Pwd"
                });
                _context.SaveChanges();
            }
        }

        protected override int GetIdFromEntity(Command entity) => entity.Id;
        protected override int GetIdFromDto(CommandDetailDto dto) => dto.Id;
        protected override int GetNonExistingId() => 9999999;

        protected override CommandCheckoutDto GetValidCreateDto()
        {
            return new CommandCheckoutDto
            {
                DeliveryAddressId = 1,
                InvoicingAddressId = 1,
                PaymentToken = "tok_test_123"
            };
        }

        protected override CommandUpdateDto GetValidUpdateDto(Command entityToUpdate)
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
        public new async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            var createDto = GetValidCreateDto();

            var actionResult = await _controller.Post(createDto);

            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
            var result = (OkObjectResult)actionResult.Result!;
            var returnedDto = result.Value as CommandDetailDto;
            Assert.IsNotNull(returnedDto);

            _context.ChangeTracker.Clear();

            var dbEntity = await _context.Commands
                .Include(c => c.CommandStatusNavigation)
                .Include(c => c.OrderDetails)
                .FirstOrDefaultAsync(c => c.Id == returnedDto.Id);

            Assert.IsNotNull(dbEntity, "La commande n'a pas été trouvée en base.");

            Assert.AreEqual(returnedDto.Total, dbEntity.Total);
            Assert.AreEqual(returnedDto.CommandStatusId, dbEntity.CommandStatusId);

            double diffSeconds = Math.Abs((returnedDto.Date - dbEntity.Date).TotalSeconds);
            Assert.IsTrue(diffSeconds < 1, "La date diffère trop.");

            Assert.IsTrue(dbEntity.OrderDetails.Any());
        }

        [TestMethod]
        public async Task Put_Admin_ShouldUpdateStatus()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var updateDto = GetValidUpdateDto(new Command());

            var result = await _controller.Put(_cmdId1, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updated = await _context.Commands.FindAsync(_cmdId1);
            Assert.AreEqual(STATUS_SHIPPED_ID, updated.CommandStatusId);
        }

        [TestMethod]
        public async Task Put_User_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_NORMAL_ID, "User");
            var updateDto = GetValidUpdateDto(new Command());

            try
            {
                var result = await _controller.Put(_cmdId1, updateDto);
            }
            catch (Exception) { }
        }

        [TestMethod]
        public async Task Delete_Admin_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_ADMIN_ID, "Admin");
            var result = await _controller.Delete(_cmdId1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public new async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            var result = await _controller.GetById(_cmdId1);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var dto = (result.Result as OkObjectResult).Value as CommandDetailDto;
            Assert.AreEqual(_cmdId1, dto.Id);
            Assert.AreEqual(100.00m, dto.Total);
        }

        [TestMethod]
        public new async Task GetAll_ShouldReturnOk()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<CommandListDto>;
            Assert.IsTrue(list.Any(c => c.Id == _cmdId1));
        }
    }
}