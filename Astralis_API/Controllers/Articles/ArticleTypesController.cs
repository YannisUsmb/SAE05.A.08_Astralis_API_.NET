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
    [DisplayName("ArticleType")]
    public class ArticleTypesController : ReadableController<ArticleType, ArticleTypeDto, ArticleTypeDto, int>
    {
        public ArticleTypesController(IArticleTypeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}