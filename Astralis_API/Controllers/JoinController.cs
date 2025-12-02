using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Astralis_API.Models.Repository;

namespace Astralis_API.Controllers
{
    /// <summary>
    /// Controller responsible for managing {EntityName} join entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class JoinController<TEntity, TDto, TKey1, TKey2> : ControllerBase
        where TEntity : class
        where TDto : class
    {
        protected readonly IJoinRepository<TEntity, TKey1, TKey2> _repository;
        protected readonly IMapper _mapper;

        public JoinController(IJoinRepository<TEntity, TKey1, TKey2> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all {EntityName} associations.
        /// </summary>
        /// <returns>A list of {EntityName}s.</returns>
        /// <response code="200">The list of {EntityName}s was successfully retrieved.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet]
        [ActionName("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            IEnumerable<TEntity?> entities = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<TDto>>(entities));
        }

        /// <summary>
        /// Retrieves a specific {EntityName} by its composite keys.
        /// </summary>
        /// <param name="id1">The first part of the composite key.</param>
        /// <param name="id2">The second part of the composite key.</param>
        /// <returns>The requested {EntityName}.</returns>
        /// <response code="200">The {EntityName} was successfully retrieved.</response>
        /// <response code="404">The {EntityName} does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("{id1}/{id2}")]
        [ActionName("GetById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<TDto>> GetById(TKey1 id1, TKey2 id2)
        {
            var entity = await _repository.GetByIdAsync(id1, id2);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<TDto>(entity));
        }

        /// <summary>
        /// Creates a new {EntityName} association.
        /// </summary>
        /// <param name="dto">The object containing the details of the {EntityName} to create.</param>
        /// <returns>A status 200 OK upon success.</returns>
        /// <response code="200">The {EntityName} was successfully created.</response>
        /// <response code="400">The input data is invalid.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPost]
        [ActionName("Post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult> Post(TDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
            return Ok();
        }

        /// <summary>
        /// Deletes a specific {EntityName} association.
        /// </summary>
        /// <param name="id1">The first part of the composite key.</param>
        /// <param name="id2">The second part of the composite key.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The {EntityName} was successfully deleted.</response>
        /// <response code="404">The {EntityName} does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id1}/{id2}")]
        [ActionName("Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> Delete(TKey1 id1, TKey2 id2)
        {
            var entity = await _repository.GetByIdAsync(id1, id2);
            if (entity == null) return NotFound();
            await _repository.DeleteAsync(entity);
            return NoContent();
        }
    }
}