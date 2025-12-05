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
    [DisplayName("ProductCategory")]
    public class ProductCategoriesController : ReadableController<ProductCategory, ProductCategoryDto, ProductCategoryDto, int>
    {
        public ProductCategoriesController(IProductCategoryRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}