using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class CartItemsControllerTests
        : JoinControllerTests<CartItemsController, CartItem, CartItemDto, CartItemCreateDto, int, int>
    {
        private const int BUYER_ID = 5005;
        private const int OTHER_BUYER_ID = 6006;
        private const int SELLER_ID = 7007;

        private const int CAT_ID = 10;

        private const int PRODUCT_ID_1 = 101;
        private const int PRODUCT_ID_2 = 102;

        protected override CartItemsController CreateController(AstralisDbContext context, IMapper mapper)
        {
            var repository = new CartItemManager(context);
            var controller = new CartItemsController(repository, mapper);

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

        protected override List<CartItem> GetSampleEntities()
        {
            _context.ChangeTracker.Clear();

            if (!_context.UserRoles.Any(r => r.Id == 1))
                _context.UserRoles.Add(new UserRole { Id = 1, Label = "User" });

            if (!_context.Users.Any(u => u.Id == BUYER_ID))
                _context.Users.Add(CreateUser(BUYER_ID, "Buyer"));

            if (!_context.Users.Any(u => u.Id == OTHER_BUYER_ID))
                _context.Users.Add(CreateUser(OTHER_BUYER_ID, "Other"));

            if (!_context.Users.Any(u => u.Id == SELLER_ID))
                _context.Users.Add(CreateUser(SELLER_ID, "Seller"));

            if (!_context.ProductCategories.Any(c => c.Id == CAT_ID))
            {
                _context.ProductCategories.Add(new ProductCategory { Id = CAT_ID, Label = "Electronics" });
            }

            if (!_context.Products.Any(p => p.Id == PRODUCT_ID_1))
            {
                _context.Products.Add(new Product
                {
                    Id = PRODUCT_ID_1,
                    Label = "Laptop",
                    Description = "Gaming Laptop",
                    Price = 1500,
                    ProductCategoryId = CAT_ID,
                    UserId = SELLER_ID
                });
            }

            if (!_context.Products.Any(p => p.Id == PRODUCT_ID_2))
            {
                _context.Products.Add(new Product
                {
                    Id = PRODUCT_ID_2,
                    Label = "Mouse",
                    Description = "Optical Mouse",
                    Price = 50,
                    ProductCategoryId = CAT_ID,
                    UserId = SELLER_ID
                });
            }

            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            var cartItem = new CartItem
            {
                UserId = BUYER_ID,
                ProductId = PRODUCT_ID_1,
                Quantity = 1,

                UserNavigation = _context.Users.Find(BUYER_ID)!,
                ProductNavigation = _context.Products.Find(PRODUCT_ID_1)!
            };

            return new List<CartItem> { cartItem };
        }

        private User CreateUser(int id, string name)
        {
            return new User
            {
                Id = id,
                LastName = name,
                FirstName = "Test",
                Email = $"{name}@test.com",
                Username = name,
                UserRoleId = 1,
                AvatarUrl = "http://url.com",
                Password = "pwd"
            };
        }

        protected override int GetKey1(CartItem entity) => entity.UserId;
        protected override int GetKey2(CartItem entity) => entity.ProductId;

        protected override int GetNonExistingKey1() => BUYER_ID;

        protected override int GetNonExistingKey2() => 99999;

        protected override CartItemCreateDto GetValidCreateDto()
        {
            return new CartItemCreateDto
            {
                ProductId = PRODUCT_ID_2,
                Quantity = 5
            };
        }

        protected override (int, int) GetKeysFromCreateDto(CartItemCreateDto dto)
        {
            return (BUYER_ID, dto.ProductId);
        }

        [TestMethod]
        public async Task Put_UpdateOwnCart_ShouldSuccess()
        {
            // Given
            var updateDto = new CartItemUpdateDto { Quantity = 10 };

            // When
            var result = await _controller.Put(BUYER_ID, PRODUCT_ID_1, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            _context.ChangeTracker.Clear();
            var item = await _context.CartItems.FindAsync(BUYER_ID, PRODUCT_ID_1);
            Assert.IsNotNull(item);
            Assert.AreEqual(10, item.Quantity);
        }

        [TestMethod]
        public async Task Put_OtherUserCart_ShouldFail_Forbidden()
        {
            // Given
            var updateDto = new CartItemUpdateDto { Quantity = 10 };

            // When
            var result = await _controller.Put(OTHER_BUYER_ID, PRODUCT_ID_1, updateDto);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_OtherUserCart_ShouldFail_Forbidden()
        {
            // Given

            // When
            var result = await _controller.Delete(OTHER_BUYER_ID, PRODUCT_ID_1);

            // Then
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }
    }
}