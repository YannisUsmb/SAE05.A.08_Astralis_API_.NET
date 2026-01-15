using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public class TypesOfArticleControllerTestsMock : JoinControllerMockTests<TypesOfArticleController, TypeOfArticle, TypeOfArticleDto, TypeOfArticleDto, int, int>
    {
        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
        }

        protected override TypesOfArticleController CreateController(Mock<IJoinRepository<TypeOfArticle, int, int>> mockRepo, AutoMapper.IMapper mapper)
        {
            _mockRepository = mockRepo;
            return new TypesOfArticleController(mockRepo.Object, mapper);
        }

        protected override int GetExistingKey1() => 1;
        protected override int GetExistingKey2() => 10;

        protected override int GetNonExistingKey1() => 99;
        protected override int GetNonExistingKey2() => 99;

        protected override TypeOfArticle GetSampleEntity() => new TypeOfArticle
        {
            ArticleTypeId = 1,
            ArticleId = 10
        };

        protected override List<TypeOfArticle> GetSampleEntities() => new List<TypeOfArticle>
        {
            new TypeOfArticle { ArticleTypeId = 1, ArticleId = 10 },
            new TypeOfArticle { ArticleTypeId = 2, ArticleId = 15 }
        };

        protected override TypeOfArticleDto GetValidCreateDto() => new TypeOfArticleDto
        {
            ArticleTypeId = 3,
            ArticleId = 20
        };

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            var createDto = GetValidCreateDto();

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TypeOfArticle>())).Returns(Task.CompletedTask);

            // When
            var result = await _controller.Post(createDto);

            // Then
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<TypeOfArticle>()), Times.Once);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOfType(okResult.Value, typeof(TypeOfArticleDto));
        }
    }
}