using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.DataManager;
using Astralis_API.Models.EntityFramework;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public class ProductCategoriesControllerTests : ReadableControllerTests<ProductCategoriesController, ProductCategory, ProductCategoryDto, ProductCategoryDto, int>
    {
        protected override ProductCategoriesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new ProductCategoriesController(new ProductCategoryManager(context), mapper);
        }

        protected override List<ProductCategory> GetSampleEntities()
        {
            return new List<ProductCategory>
            {
                new ProductCategory {Id=902101, Label = "Report motive 1", Description = "ProductCategory description 1"},
                new ProductCategory {Id=902102, Label = "Report motive 2", Description = "ProductCategory description 2"}
            };
        }

        protected override int GetIdFromEntity(ProductCategory entity)
        {
            return entity.Id;
        }

        protected override int GetNonExistingId()
        {
            return -1;
        }
    }
}