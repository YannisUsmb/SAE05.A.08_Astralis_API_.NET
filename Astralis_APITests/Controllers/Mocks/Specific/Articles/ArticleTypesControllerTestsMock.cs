using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class ArticleTypesControllerTestsMock : CrudControllerMockTests<ArticleTypesController, ArticleType, ArticleTypeDto, ArticleTypeDto, ArticleTypeDto, ArticleTypeDto, int>
    {
        private Mock<IArticleTypeRepository> _mockArticleTypeRepository;

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
        }

        protected override ArticleTypesController CreateController(Mock<IReadableRepository<ArticleType, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockArticleTypeRepository = new Mock<IArticleTypeRepository>();
            _mockRepository = _mockArticleTypeRepository.As<IReadableRepository<ArticleType, int>>();

            _mockCrudRepository = _mockArticleTypeRepository.As<ICrudRepository<ArticleType, int>>();

            return new ArticleTypesController(_mockArticleTypeRepository.Object, mapper);
        }

        protected override void SetIdInUpdateDto(ArticleTypeDto dto, int id)
        {
            dto.Id = id;
        }

        protected override List<ArticleType> GetSampleEntities() => new List<ArticleType>
        {
            new ArticleType { Id = 1, Label = "Actualités", Description = "Articles sur l'actualité spatiale" },
            new ArticleType { Id = 2, Label = "Dossiers", Description = "Analyses approfondies" }
        };

        protected override ArticleType GetSampleEntity() => new ArticleType
        {
            Id = 1,
            Label = "Actualités",
            Description = "Articles sur l'actualité spatiale"
        };

        protected override int GetExistingId() => 1;
        protected override int GetNonExistingId() => 999;

        protected override ArticleTypeDto GetValidCreateDto() => new ArticleTypeDto
        {
            Label = "Tutoriels",
            Description = "Guides pratiques pour l'observation"
        };

        protected override ArticleTypeDto GetValidUpdateDto() => new ArticleTypeDto
        {
            Id = 1,
            Label = "Actualités (Mis à jour)",
            Description = "Description modifiée"
        };

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnCreated()
        {
            // Given
            var createDto = GetValidCreateDto();

            _mockArticleTypeRepository.Setup(r => r.AddAsync(It.IsAny<ArticleType>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockArticleTypeRepository.Verify(r => r.AddAsync(It.IsAny<ArticleType>()), Times.Once);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOfType(okResult.Value, typeof(ArticleTypeDto));
        }
    }
}