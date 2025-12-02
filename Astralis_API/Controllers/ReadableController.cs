using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Astralis_API.Models.Repository;

namespace Astralis_API.Controllers
{
    /// <summary>
    /// Controller responsible for managing {EntityName} readable entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ReadableController<TEntity, TGetAllDto, TGetDto, TId> : ControllerBase
        where TEntity : class
        where TGetAllDto : class
        where TGetDto : class
    {
        protected readonly IReadableRepository<TEntity, TId> _repository;
        protected readonly IMapper _mapper;

        public ReadableController(IReadableRepository<TEntity, TId> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all {EntityNamePlural}.
        /// </summary>
        /// <returns>A list of {EntityNamePlural}.</returns>
        /// <response code="200">The list of {EntityNamePlural} was successfully retrieved.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet]
        [ActionName("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<IEnumerable<TGetAllDto>>> GetAll()
        {
            IEnumerable<TEntity> entities = await _repository.GetAllAsync();
            IEnumerable<TGetAllDto> dtos = _mapper.Map<IEnumerable<TGetAllDto>>(entities);
            
            return Ok(dtos);
        }

        /// <summary>
        /// Retrieves a specific {EntityName} by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the {EntityName}.</param>
        /// <returns>The requested {EntityName}.</returns>
        /// <response code="200">The {EntityName} was successfully retrieved.</response>
        /// <response code="404">The {EntityName} does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("{id}")]
        [ActionName("GetById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<ActionResult<TGetDto>> GetById(TId id)
        {
            TEntity? entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return NotFound();

            TGetDto? dto = _mapper.Map<TGetDto>(entity);

            return Ok(dto);
        }
    }
}