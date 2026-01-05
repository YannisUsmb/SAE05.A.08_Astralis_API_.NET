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
    public class OrderDetailsControllerTests
        : JoinControllerTests<OrderDetailsController, OrderDetail, OrderDetailDto, OrderDetailCreateDto, int, int>
    {
        // --- CONSTANTES ---
        private const int BUYER_ID = 5005;
        private const int OTHER_BUYER_ID = 6006;
        private const int SELLER_ID = 7007;

        private const int STATUS_OPEN_ID = 1;      // Statut permettant la suppression
        private const int STATUS_SHIPPED_ID = 2;   // Statut interdisant la suppression

        private const int COMMAND_ID_1 = 8001;     // Commande de BUYER (Status Open)
        private const int COMMAND_ID_2 = 8002;     // Commande de OTHER (Status Open)
        private const int COMMAND_ID_LOCKED = 8003;// Commande de BUYER (Status Shipped)

        private const int PRODUCT_ID_1 = 101;
        private const int PRODUCT_ID_2 = 102;      // Produit pour le test POST
        private const int CATEGORY_ID = 10;

        // --- 1. CONFIGURATION ---
        protected override OrderDetailsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var orderDetailRepo = new OrderDetailManager(context);
            var commandRepo = new CommandManager(context);

            var controller = new OrderDetailsController(orderDetailRepo, commandRepo, mapper);

            // Simulation Utilisateur Connecté (BUYER_ID)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, BUYER_ID.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            return controller;
        }

        // --- 2. JEU DE DONNÉES (SEED) ---
        protected override List<OrderDetail> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // A. Users
            if (!_context.Users.Any(u => u.Id == BUYER_ID)) _context.Users.Add(CreateUser(BUYER_ID, "Buyer"));
            if (!_context.Users.Any(u => u.Id == OTHER_BUYER_ID)) _context.Users.Add(CreateUser(OTHER_BUYER_ID, "Other"));
            if (!_context.Users.Any(u => u.Id == SELLER_ID)) _context.Users.Add(CreateUser(SELLER_ID, "Seller"));

            // B. Dependencies (Category, Status)
            if (!_context.ProductCategories.Any(c => c.Id == CATEGORY_ID))
                _context.ProductCategories.Add(new ProductCategory { Id = CATEGORY_ID, Label = "General" });

            if (!_context.CommandStatuses.Any(s => s.Id == STATUS_OPEN_ID))
                _context.CommandStatuses.Add(new CommandStatus { Id = STATUS_OPEN_ID, Label = "New" });

            if (!_context.CommandStatuses.Any(s => s.Id == STATUS_SHIPPED_ID))
                _context.CommandStatuses.Add(new CommandStatus { Id = STATUS_SHIPPED_ID, Label = "Shipped" });

            // C. Products
            if (!_context.Products.Any(p => p.Id == PRODUCT_ID_1))
                _context.Products.Add(CreateProduct(PRODUCT_ID_1));

            if (!_context.Products.Any(p => p.Id == PRODUCT_ID_2))
                _context.Products.Add(CreateProduct(PRODUCT_ID_2));

            _context.SaveChanges();

            // D. Commands (CORRIGÉ : DateTime.UtcNow au lieu de DateTime.Now)

            // Commande 1 : Ouverte, appartient à l'acheteur
            if (!_context.Commands.Any(c => c.Id == COMMAND_ID_1))
            {
                _context.Commands.Add(new Command
                {
                    Id = COMMAND_ID_1,
                    UserId = BUYER_ID,
                    CommandStatusId = STATUS_OPEN_ID,
                    Date = DateTime.UtcNow,    // <--- CORRECTION ICI
                    Total = 150.00m,
                    PdfName = "invoice_1.pdf",
                    PdfPath = "/docs/1.pdf"
                });
            }

            // Commande 2 : Ouverte, appartient à un autre
            if (!_context.Commands.Any(c => c.Id == COMMAND_ID_2))
            {
                _context.Commands.Add(new Command
                {
                    Id = COMMAND_ID_2,
                    UserId = OTHER_BUYER_ID,
                    CommandStatusId = STATUS_OPEN_ID,
                    Date = DateTime.UtcNow,    // <--- CORRECTION ICI
                    Total = 200.00m,
                    PdfName = "invoice_2.pdf",
                    PdfPath = "/docs/2.pdf"
                });
            }

            // Commande 3 : Fermée/Expédiée, appartient à l'acheteur
            if (!_context.Commands.Any(c => c.Id == COMMAND_ID_LOCKED))
            {
                _context.Commands.Add(new Command
                {
                    Id = COMMAND_ID_LOCKED,
                    UserId = BUYER_ID,
                    CommandStatusId = STATUS_SHIPPED_ID,
                    Date = DateTime.UtcNow,    // <--- CORRECTION ICI
                    Total = 50.00m,
                    PdfName = "invoice_3.pdf",
                    PdfPath = "/docs/3.pdf"
                });
            }

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            // E. Join Entity (OrderDetail)
            // On s'assure que les navigations sont chargées pour éviter les NullReferenceException
            var command1 = _context.Commands
                .Include(c => c.CommandStatusNavigation)
                .First(c => c.Id == COMMAND_ID_1);

            var product1 = _context.Products.Find(PRODUCT_ID_1);

            var orderDetail = new OrderDetail
            {
                CommandId = COMMAND_ID_1,
                ProductId = PRODUCT_ID_1,
                Quantity = 5,

                // Navigations requises pour AutoMapper et Controller checks
                CommandNavigation = command1,
                ProductNavigation = product1!
            };

            return new List<OrderDetail> { orderDetail };
        }

        // Helpers
        private User CreateUser(int id, string name) => new User
        {
            Id = id,
            LastName = name,
            FirstName = "Test",
            Email = $"{name}@test.com",
            Username = name,
            UserRoleId = 1,
            AvatarUrl = "url",
            Password = "pwd"
        };

        private Product CreateProduct(int id) => new Product
        {
            Id = id,
            Label = $"Prod {id}",
            Price = 10,
            Description = "Desc",
            ProductCategoryId = CATEGORY_ID,
            UserId = SELLER_ID
        };

        // --- 3. IMPLÉMENTATIONS ABSTRAITES ---

        protected override int GetKey1(OrderDetail entity) => entity.CommandId;
        protected override int GetKey2(OrderDetail entity) => entity.ProductId;

        // IDs inexistants pour le test NotFound
        protected override int GetNonExistingKey1() => 9999;
        protected override int GetNonExistingKey2() => 9999;

        protected override OrderDetailCreateDto GetValidCreateDto()
        {
            // Given: Ajouter le produit 2 à la commande 1 (Ouverte et appartenant à User)
            return new OrderDetailCreateDto
            {
                CommandId = COMMAND_ID_1,
                ProductId = PRODUCT_ID_2,
                Quantity = 3
            };
        }

        protected override (int, int) GetKeysFromCreateDto(OrderDetailCreateDto dto)
        {
            return (dto.CommandId, dto.ProductId);
        }

        // =========================================================================================
        // TESTS SPÉCIFIQUES (Sécurité & Logique Métier)
        // =========================================================================================

        [TestMethod]
        public async Task Delete_OrderOfOtherUser_ShouldFail_Forbidden()
        {
            // Given: On insère un détail dans la commande d'un AUTRE utilisateur
            var detail = new OrderDetail
            {
                CommandId = COMMAND_ID_2,
                ProductId = PRODUCT_ID_1,
                Quantity = 1
            };
            _context.OrderDetails.Add(detail);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // When: L'utilisateur connecté (BUYER_ID) essaie de supprimer le détail de OTHER_BUYER_ID
            var result = await _controller.Delete(COMMAND_ID_2, PRODUCT_ID_1);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_ProcessedOrder_ShouldFail_BadRequest()
        {
            // Given: On insère un détail dans une commande EXPÉDIÉE (Statut 2) de l'utilisateur
            var detail = new OrderDetail
            {
                CommandId = COMMAND_ID_LOCKED,
                ProductId = PRODUCT_ID_1,
                Quantity = 1
            };
            _context.OrderDetails.Add(detail);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // When: L'utilisateur essaie de supprimer une ligne d'une commande déjà traitée
            var result = await _controller.Delete(COMMAND_ID_LOCKED, PRODUCT_ID_1);

            // Then
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            // Vérification du message d'erreur si nécessaire
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("Cannot delete items from a processed order.", badRequest?.Value);
        }
    }
}