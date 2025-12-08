using Astralis.Shared.DTOs;
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
    [DisplayName("Comment")]
    public class CommentsController : CrudController<Comment, CommentDto, CommentDto, CommentCreateDto, CommentUpdateDto, int>
    {
        public CommentsController(ICommentRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        /// <summary>
        /// Retrieves all comments (public access).
        /// </summary>
        /// <returns>A list of comments.</returns>
        /// <response code="200">Returns the list of comments.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<CommentDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific comment by ID (public access).
        /// </summary>
        /// <param name="id">The unique identifier of the comment.</param>
        /// <returns>The requested comment.</returns>
        /// <response code="200">Returns the comment.</response>
        /// <response code="404">Comment not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<CommentDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Creates a new Comment linked to the authenticated user.
        /// </summary>
        /// <param name="createDto">The comment content and article ID.</param>
        /// <returns>The newly created comment.</returns>
        /// <response code="200">The comment was successfully created.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<CommentDto>> Post(CommentCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            Comment entity = _mapper.Map<Comment>(createDto);

            entity.UserId = userId; 
            entity.Date = DateTime.UtcNow;
            entity.IsVisible = true;

            await _repository.AddAsync(entity);

            CommentDto returnDto = _mapper.Map<CommentDto>(entity);
            return Ok(returnDto);
        }

        /// <summary>
        /// Updates an existing comment text.
        /// </summary>
        /// <remarks>Users can only update their own comments.</remarks>
        /// <param name="id">The ID of the comment to update.</param>
        /// <param name="updateDto">The new text content.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The comment was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User tried to modify a comment that belongs to someone else.</response>
        /// <response code="404">Comment not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, CommentUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Comment? entityToUpdate = await _repository.GetByIdAsync(id);
            if (entityToUpdate == null)
                return NotFound();

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || entityToUpdate.UserId != userId)
            {
                return Forbid();
            }

            _mapper.Map(updateDto, entityToUpdate);

            await _repository.UpdateAsync(entityToUpdate, entityToUpdate);

            return NoContent();
        }

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="id">The unique identifier of the comment to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The comment was successfully deleted.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User is not authorized (not the author or admin).</response>
        /// <response code="404">The comment does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id)
        {
            Comment? entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            if (entity.UserId != userId && userRole != "Admin")
            {
                return Forbid();
            }

            return await base.Delete(id);
        }
    }
}