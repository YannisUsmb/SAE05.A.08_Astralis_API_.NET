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
    [DisplayName("Address")]
    public class AddressesController : CrudController<Address, AddressDto, AddressDto, AddressCreateDto, AddressUpdateDto, int>
    {
        public AddressesController(IAddressRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}