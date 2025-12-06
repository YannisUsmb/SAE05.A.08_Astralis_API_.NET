using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("Asteroid")]
    public class AsteroidsController : CrudController<Asteroid, AsteroidDto, AsteroidDto, AsteroidCreateDto, AsteroidUpdateDto, int>
    {
        private readonly IAsteroidRepository _asteroidRepository;

        public AsteroidsController(IAsteroidRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _asteroidRepository = repository;
        }

        /// <summary>
        /// Retrieves all asteroids (public access).
        /// </summary>
        /// <returns>A list of all asteroids.</returns>
        /// <response code="200">Returns the list of asteroids.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<AsteroidDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific asteroid by ID (public access).
        /// </summary>
        /// <param name="id">The asteroid ID.</param>
        /// <returns>The detailed asteroid.</returns>
        /// <response code="200">Returns the asteroid.</response>
        /// <response code="404">Asteroid not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<AsteroidDto>> GetById(int id)
        {
            return base.GetById(id);
        }

        /// <summary>
        /// Searches for asteroids based on physical and orbital parameters.
        /// </summary>
        /// <param name="filter">The search criteria (e.g., hazard status, magnitude, diameter).</param>
        /// <returns>A list of matching asteroids.</returns>
        /// <response code="200">List retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AsteroidDto>>> Search([FromQuery] AsteroidFilterDto filter)
        {
            IEnumerable<Asteroid?> asteroids = await _asteroidRepository.SearchAsync(
                filter.Reference,
                filter.OrbitalClassIds,
                filter.IsPotentiallyHazardous,
                filter.OrbitId,
                filter.MinAbsoluteMagnitude,
                filter.MaxAbsoluteMagnitude,
                filter.MinDiameter,
                filter.MaxDiameter,
                filter.MinInclination,
                filter.MaxInclination,
                filter.MinSemiMajorAxis,
                filter.MaxSemiMajorAxis
            );

            return Ok(_mapper.Map<IEnumerable<AsteroidDto>>(asteroids));
        }
    }
}