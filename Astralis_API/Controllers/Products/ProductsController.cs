using Astralis.Shared.DTOs;
using Astralis_API.Configuration;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("Product")]
    public class ProductsController : CrudController<Product, ProductListDto, ProductDetailDto, ProductCreateDto, ProductUpdateDto, int>
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _productRepository = repository;
        }

        /// <summary>
        /// Retrieves all products (public access).
        /// </summary>
        /// <returns>A list of products.</returns>
        /// <response code="200">Returns the list of products.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<ProductListDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific product by ID (public access).
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The detailed product.</returns>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">Product not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<ProductDetailDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for products based on text, category, or price range.
        /// </summary>
        /// <param name="filter">The search criteria.</param>
        /// <returns>A list of matching products.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> Search([FromQuery] ProductFilterDto filter)
        {
            IEnumerable<Product?> products = await _productRepository.SearchAsync(
                filter.SearchText,
                filter.ProductCategoryIds,
                filter.MinPrice,
                filter.MaxPrice
            );
            return Ok(_mapper.Map<IEnumerable<ProductListDto>>(products));
        }

        /// <summary>
        /// Creates a new product (Commercial Editor only).
        /// </summary>
        /// <param name="createDto">The product data.</param>
        /// <returns>The created product.</returns>
        /// <response code="200">Product created successfully.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User not authorized (requires Commercial Editor role).</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<ProductDetailDto>> Post(ProductCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            Product entity = _mapper.Map<Product>(createDto);
            entity.UserId = userId;

            await _repository.AddAsync(entity);
            return Ok(_mapper.Map<ProductDetailDto>(entity));
        }

        /// <summary>
        /// Updates an existing product (Commercial Editor only).
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="updateDto">Updated data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Product updated successfully.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not the owner of this product.</response>
        /// <response code="404">Product not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, ProductUpdateDto updateDto)
        {
            Product? entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || entity.UserId != userId)
            {
                return Forbid();
            }

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes a product (Commercial Editor only).
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The product was successfully deleted.</response>
        /// <response code="404">The product does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            Product? entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || entity.UserId != userId)
            {
                return Forbid();
            }

            return await base.Delete(id);
        }
    }
}