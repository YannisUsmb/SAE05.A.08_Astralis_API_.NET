using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CitiesController : CrudController<City, CityDto, CityDto, CityCreateDto, CityCreateDto, int>
{
    private readonly ICityRepository _cityRepository;

    public CitiesController(ICityRepository repository, IMapper mapper)
        : base(repository, mapper)
    {
        _cityRepository = repository;
    }

    /// <summary>
    /// Retrieves a list of cities matching the specified search term.
    /// </summary>
    /// <param name="term">The term (name or postal code) to search for.</param>
    /// <returns>A list of matching cities.</returns>
    /// <response code="200">The cities were successfully retrieved.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("search/{term}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CityDto>>> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
        {
            return Ok(new List<CityDto>());
        }

        try
        {
            var cities = await _cityRepository.SearchAsync(term);
            var citiesDto = _mapper.Map<IEnumerable<CityDto>>(cities);

            return Ok(citiesDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error during search.");
        }
    }
}