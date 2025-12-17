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
    public class ProductsControllerTests
        : CrudControllerTests<ProductsController, Product, ProductListDto, ProductDetailDto, ProductCreateDto, ProductUpdateDto, int>
    {
        // --- CONSTANTES ---
        private const int USER_EDITOR_ID = 5001; // Rédacteur commercial (Owner)
        private const int USER_OTHER_ID = 5002;  // Autre utilisateur

        private const int CAT_TECH_ID = 1;
        private const int CAT_BOOK_ID = 2;

        private const int PROD_ID_1 = 33001;
        private const int PROD_ID_2 = 33002;

        // Variables membres
        private int _prodId1;
        private int _prodId2;

        protected override ProductsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var productManager = new ProductManager(context);

            var controller = new ProductsController(
                productManager,
                mapper
            );

            SetupUserContext(controller, USER_EDITOR_ID, "Rédacteur commercial");
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

        protected override List<Product> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            // ==============================================================================
            // 1. SETUP DES DEPENDANCES (Sauvegarde en base)
            // ==============================================================================

            // Rôles
            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == 10))
                _context.UserRoles.Add(new UserRole { Id = 10, Label = "Rédacteur commercial" });

            if (!_context.UserRoles.AsNoTracking().Any(r => r.Id == 1))
                _context.UserRoles.Add(new UserRole { Id = 1, Label = "User" });

            // Users
            CreateUserIfNotExist(USER_EDITOR_ID, 10);
            CreateUserIfNotExist(USER_OTHER_ID, 10);

            // Categories
            if (!_context.ProductCategories.AsNoTracking().Any(c => c.Id == CAT_TECH_ID))
                _context.ProductCategories.Add(new ProductCategory { Id = CAT_TECH_ID, Label = "Technology" });

            if (!_context.ProductCategories.AsNoTracking().Any(c => c.Id == CAT_BOOK_ID))
                _context.ProductCategories.Add(new ProductCategory { Id = CAT_BOOK_ID, Label = "Books" });

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            // ==============================================================================
            // 2. RECUPERATION DES DEPENDANCES (Fetching)
            // On récupère les objets suivis par le contexte pour éviter l'erreur "Duplicate Key"
            // ==============================================================================

            var catTech = _context.ProductCategories.Find(CAT_TECH_ID);
            var catBook = _context.ProductCategories.Find(CAT_BOOK_ID);

            // On inclut le rôle pour que l'objet User soit complet pour AutoMapper si nécessaire
            var userEditor = _context.Users.Include(u => u.UserRoleNavigation).FirstOrDefault(u => u.Id == USER_EDITOR_ID);
            var userOther = _context.Users.Include(u => u.UserRoleNavigation).FirstOrDefault(u => u.Id == USER_OTHER_ID);

            // ==============================================================================
            // 3. CREATION DES PRODUITS EN MÉMOIRE
            // ==============================================================================

            var p1 = new Product
            {
                Id = PROD_ID_1,
                Label = "Telescope X2000",
                Description = "See the stars",
                Price = 1200.50m,
                UserId = USER_EDITOR_ID,
                ProductCategoryId = CAT_TECH_ID,

                // On lie les objets RÉCUPÉRÉS (Tracked)
                // EF Core ne tentera pas de les réinsérer, et AutoMapper aura les données pour les assertions.
                UserNavigation = userEditor!,
                ProductCategoryNavigation = catTech!
            };

            var p2 = new Product
            {
                Id = PROD_ID_2,
                Label = "Astronomy Guide",
                Description = "Learn everything",
                Price = 45.00m,
                UserId = USER_OTHER_ID,
                ProductCategoryId = CAT_BOOK_ID,

                UserNavigation = userOther!,
                ProductCategoryNavigation = catBook!
            };

            _prodId1 = PROD_ID_1;
            _prodId2 = PROD_ID_2;

            return new List<Product> { p1, p2 };
        }

        // --- HELPER DEPENDANCES ---
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
            }
        }

        // --- CONFIGURATION CRUD ---
        protected override int GetIdFromEntity(Product entity) => entity.Id;

        // CORRECTION : Le contrôleur renvoie ProductDetailDto pour GetById
        protected override int GetIdFromDto(ProductDetailDto dto) => dto.Id;

        protected override int GetNonExistingId() => 9999999;

        protected override ProductCreateDto GetValidCreateDto()
        {
            return new ProductCreateDto
            {
                Label = "New Product",
                Description = "Description",
                Price = 99.99m,
                ProductCategoryId = CAT_TECH_ID
            };
        }

        protected override ProductUpdateDto GetValidUpdateDto(Product entityToUpdate)
        {
            return new ProductUpdateDto
            {
                Label = "Updated Product Label",
                Description = "Updated Description",
                Price = 150.00m,
                ProductCategoryId = CAT_TECH_ID
            };
        }

        protected override void SetIdInUpdateDto(ProductUpdateDto dto, int id) { }

        // =========================================================================================
        // TESTS SPECIFIQUES
        // =========================================================================================

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreate_ReturnsCreated()
        {
            await base.Post_ValidObject_ShouldCreateAndReturn200();
        }

        // --- TESTS DELETE & PUT (Sécurité) ---

        [TestMethod]
        public async Task Delete_Owner_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_EDITOR_ID, "Rédacteur commercial");
            var result = await _controller.Delete(_prodId1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Delete_OtherUser_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_EDITOR_ID, "Rédacteur commercial");
            var result = await _controller.Delete(_prodId2);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Put_Owner_ShouldSuccess()
        {
            SetupUserContext(_controller, USER_EDITOR_ID, "Rédacteur commercial");
            var updateDto = GetValidUpdateDto(new Product());

            var result = await _controller.Put(_prodId1, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var updated = await _context.Products.FindAsync(_prodId1);
            Assert.AreEqual("Updated Product Label", updated.Label);
        }

        [TestMethod]
        public async Task Put_OtherUser_ShouldFail_Forbidden()
        {
            SetupUserContext(_controller, USER_EDITOR_ID, "Rédacteur commercial");
            var updateDto = GetValidUpdateDto(new Product());
            var result = await _controller.Put(_prodId2, updateDto);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        // --- TESTS SEARCH ---

        [TestMethod]
        public async Task Search_ByPriceRange_ShouldReturnCorrectProducts()
        {
            var filter = new ProductFilterDto
            {
                MinPrice = 1000m
            };

            var result = await _controller.Search(filter);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var list = (result.Result as OkObjectResult).Value as IEnumerable<ProductListDto>;

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Any(p => p.Id == _prodId1));
            Assert.IsFalse(list.Any(p => p.Id == _prodId2));
        }
    }
}