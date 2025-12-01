using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Astralis_API.Models.Repository;

namespace Astralis_API.Controllers.Generic
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ReadableController<TEntity, TDto, TId> : ControllerBase
        where TEntity : class
        where TDto : class
    {
        protected readonly IReadableRepository<TEntity, TId> _repository;
        protected readonly IMapper _mapper;

        public ReadableController(IReadableRepository<TEntity, TId> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            var entities = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> GetById(TId id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var dto = _mapper.Map<TDto>(entity);
            return Ok(dto);
        }
    }
}