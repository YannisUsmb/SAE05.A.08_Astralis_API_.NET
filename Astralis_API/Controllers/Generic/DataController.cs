using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Astralis_API.Models.Repository;

namespace Astralis_API.Controllers.Generic
{
    public abstract class DataController<TEntity, TDto, TCreateDto, TUpdateDto, TId, TKey>
        : CrudController<TEntity, TDto, TCreateDto, TUpdateDto, TId>
        where TEntity : class
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected new IDataRepository<TEntity, TId, TKey> _repository => (IDataRepository<TEntity, TId, TKey>)base._repository;

        public DataController(IDataRepository<TEntity, TId, TKey> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        [HttpGet("search/{key}")]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetByKey(TKey key)
        {
            var entities = await _repository.GetByKeyAsync(key);
            if (entities == null || !entities.Any()) return NotFound();

            var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
            return Ok(dtos);
        }
    }
}