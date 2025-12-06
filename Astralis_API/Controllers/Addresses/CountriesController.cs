using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DisplayName("Country")]
    public class CountriesController : ReadableController<Country, CountryDto, CountryDto, int>
    {
        public CountriesController(ICountryRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}