using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Astralis_API.Models.Repository;

namespace Astralis_API.Controllers
{
    /// <summary>
    /// Controller responsible for managing {EntityName} crud entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class CrudController<TEntity, TGetAllDto, TGetDto, TCreateDto, TUpdateDto, TId>
        : ReadableController<TEntity, TGetAllDto, TGetDto, TId>
        where TEntity : class
        where TGetAllDto : class
        where TGetDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected new ICrudRepository<TEntity, TId> _repository => (ICrudRepository<TEntity, TId>)base._repository;

        public CrudController(ICrudRepository<TEntity, TId> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        /// <summary>
        /// Creates a new {EntityName}.
        /// </summary>
        /// <param name="createDto">The object containing the details of the {EntityName} to create.</param>
        /// <returns>The newly created {EntityName}.</returns>
        /// <response code="200">The {EntityName} was successfully created.</response>
        /// <response code="400">The input data is invalid.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPost]
        [ActionName("Post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<TGetDto>> Post(TCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<TEntity>(createDto);
            await _repository.AddAsync(entity);

            var returnDto = _mapper.Map<TGetDto>(entity);

            return Ok(returnDto);
        }

        /// <summary>
        /// Updates an existing {EntityName}.
        /// </summary>
        /// <param name="id">The unique identifier of the {EntityName} to update.</param>
        /// <param name="updateDto">The object containing the updated details.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The {EntityName} was successfully updated.</response>
        /// <response code="400">The input data is invalid.</response>
        /// <response code="404">The {EntityName} does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPut("{id:int}")]
        [ActionName("Put")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> Put(TId id, TUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entityToUpdate = await _repository.GetByIdAsync(id);
            if (entityToUpdate == null) return NotFound();

            _mapper.Map(updateDto, entityToUpdate);

            await _repository.UpdateAsync(entityToUpdate, entityToUpdate);

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific {EntityName}.
        /// </summary>
        /// <param name="id">The unique identifier of the {EntityName} to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The {EntityName} was successfully deleted.</response>
        /// <response code="404">The {EntityName} does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id:int}")]
        [ActionName("Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> Delete(TId id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _repository.DeleteAsync(entity);
            return NoContent();
        }
    }
}