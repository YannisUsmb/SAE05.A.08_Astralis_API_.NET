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
    [DisplayName("Phone Prefix")]
    public class PhonePrefixesController : ReadableController<PhonePrefix, PhonePrefixDto, PhonePrefixDto, int>
    {
        public PhonePrefixesController(IPhonePrefixRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}