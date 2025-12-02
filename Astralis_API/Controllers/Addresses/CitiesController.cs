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
    [DisplayName("City")]
    public class CitiesController : CrudController<City, CityDto, CityDto, CityCreateDto, CityCreateDto, int>
    {
        public CitiesController(ICityRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}