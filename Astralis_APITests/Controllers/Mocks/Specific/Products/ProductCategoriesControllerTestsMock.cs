using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class ProductCategoriesControllerTestsMock : ReadableControllerMockTests<ProductCategoriesController, ProductCategory, ProductCategoryDto, ProductCategoryDto, int>
    {
        private Mock<IProductCategoryRepository> _mockProductCategoryRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
        }

        protected override ProductCategoriesController CreateController(Mock<IReadableRepository<ProductCategory, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockProductCategoryRepository = new Mock<IProductCategoryRepository>();

            _mockRepository = _mockProductCategoryRepository.As<IReadableRepository<ProductCategory, int>>();

            return new ProductCategoriesController(_mockProductCategoryRepository.Object, mapper);
        }

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 99;

        protected override ProductCategory GetSampleEntity() => new ProductCategory
        {
            Id = 1,
            Label = "Télescopes",
            Description = "Instruments d'observation astronomique"
        };

        protected override List<ProductCategory> GetSampleEntities() => new List<ProductCategory>
        {
            new ProductCategory { Id = 1, Label = "Télescopes", Description = "Instruments d'observation" },
            new ProductCategory { Id = 2, Label = "Accessoires", Description = "Oculaires, filtres, trépieds" },
            new ProductCategory { Id = 3, Label = "Livres", Description = "Guides et cartes célestes" }
        };
    }
}