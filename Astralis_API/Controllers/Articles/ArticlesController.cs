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
    [DisplayName("Article")]
    public class ArticlesController : CrudController<Article, ArticleListDto, ArticleDetailDto, ArticleCreateDto, ArticleUpdateDto, int>
    {
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(IArticleRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _articleRepository = repository;
        }

        /// <summary>
        /// Retrieves all articles (public access).
        /// </summary>
        /// <returns>A list of articles with previews.</returns>
        /// <response code="200">Returns the list of articles.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<ArticleListDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific article by ID (public access).
        /// </summary>
        /// <param name="id">The article ID.</param>
        /// <returns>The detailed article.</returns>
        /// <response code="200">Returns the article.</response>
        /// <response code="404">Article not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<ArticleDetailDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for articles based on text, category, or premium status.
        /// </summary>
        /// <param name="filter">The search criteria.</param>
        /// <returns>A list of matching articles.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ArticleListDto>>> Search([FromQuery] ArticleFilterDto filter)
        {
            IEnumerable<Article?> articles = await _articleRepository.SearchAsync(filter.SearchTerm, filter.TypeIds, filter.IsPremium);
            return Ok(_mapper.Map<IEnumerable<ArticleListDto>>(articles));
        }

        /// <summary>
        /// Creates a new article (Commercial Editor only).
        /// </summary>
        /// <param name="createDto">The article data.</param>
        /// <returns>The created article.</returns>
        /// <response code="200">Article created successfully.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<ArticleDetailDto>> Post(ArticleCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            Article entity = _mapper.Map<Article>(createDto);
            entity.UserId = userId;

            await _repository.AddAsync(entity);
            return Ok(_mapper.Map<ArticleDetailDto>(entity));
        }

        /// <summary>
        /// Updates an existing article (Commercial Editor only).
        /// </summary>
        /// <param name="id">Article ID.</param>
        /// <param name="updateDto">Updated data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Article updated successfully.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not the author of this article.</response>
        /// <response code="404">Article not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, ArticleUpdateDto updateDto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || entity.UserId != userId)
            {
                return Forbid();
            }

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Deletes an article (Commercial Editor only).
        /// </summary>
        /// <param name="id">The unique identifier of the article to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The article was successfully deleted.</response>
        /// <response code="404">The article does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            Article? entity = await _repository.GetByIdAsync(id);
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