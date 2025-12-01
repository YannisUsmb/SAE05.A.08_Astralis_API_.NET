using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Astralis_API.Models.Repository;

namespace Astralis_API.Controllers.Generic
{
    public abstract class CrudController<TEntity, TDto, TCreateDto, TUpdateDto, TId>
        : ReadableController<TEntity, TDto, TId>
        where TEntity : class
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected new ICrudRepository<TEntity, TId> _repository => (ICrudRepository<TEntity, TId>)base._repository;

        public CrudController(ICrudRepository<TEntity, TId> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Post(TCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<TEntity>(createDto);
            await _repository.AddAsync(entity);

            var returnDto = _mapper.Map<TDto>(entity);
            return Ok(returnDto);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(TId id, TUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entityToUpdate = await _repository.GetByIdAsync(id);
            if (entityToUpdate == null) return NotFound();

            _mapper.Map(updateDto, entityToUpdate);
            await _repository.UpdateAsync(entityToUpdate, entityToUpdate);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TId id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _repository.DeleteAsync(entity);
            return NoContent();
        }
    }
}