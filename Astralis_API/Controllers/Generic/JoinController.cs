using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Astralis_API.Models.Repository;

namespace Astralis_API.Controllers.Generic
{
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

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<TDto>>(entities));
        }

        [HttpGet("{id1}/{id2}")]
        public virtual async Task<ActionResult<TDto>> GetById(TKey1 id1, TKey2 id2)
        {
            var entity = await _repository.GetByIdAsync(id1, id2);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<TDto>(entity));
        }

        [HttpPost]
        public virtual async Task<ActionResult> Post(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
            return Ok();
        }

        [HttpDelete("{id1}/{id2}")]
        public virtual async Task<IActionResult> Delete(TKey1 id1, TKey2 id2)
        {
            var entity = await _repository.GetByIdAsync(id1, id2);
            if (entity == null) return NotFound();
            await _repository.DeleteAsync(entity);
            return NoContent();
        }
    }
}