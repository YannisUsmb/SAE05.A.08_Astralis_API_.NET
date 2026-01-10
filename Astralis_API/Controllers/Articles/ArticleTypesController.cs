using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DisplayName("Article Type")]
    public class ArticleTypesController : CrudController<ArticleType, ArticleTypeDto, ArticleTypeDto, ArticleTypeDto, ArticleTypeDto, int>
    {
        public ArticleTypesController(IArticleTypeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        [HttpPost]
        [Authorize(Roles = "Rédacteur Commercial")]
        public override async Task<ActionResult<ArticleTypeDto>> Post(ArticleTypeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<ArticleType>(dto);
            await _repository.AddAsync(entity);

            var newDto = _mapper.Map<ArticleTypeDto>(entity);
            return Ok(newDto);
        }
    }
}