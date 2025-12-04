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
    [Authorize(Roles = "Admin")] 
    [DisplayName("TypeOfArticle")]
    public class TypeOfArticlesController : JoinController<TypeOfArticle, TypeOfArticleDto, int, int>
    {
        public TypeOfArticlesController(IJoinRepository<TypeOfArticle, int, int> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}