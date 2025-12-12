using Astralis_API.Controllers;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public abstract class JoinControllerMockTests<TController, TEntity, TDto, TCreateDto, TKey1, TKey2>
        : BaseControllerMockTests
        where TEntity : class
        where TController : JoinController<TEntity, TDto, TCreateDto, TKey1, TKey2>
        where TDto : class
        where TCreateDto : class
    {
        protected Mock<IJoinRepository<TEntity, TKey1, TKey2>> _mockRepository;
        protected TController _controller;

        protected abstract List<TEntity> GetSampleEntities();
        protected abstract TEntity GetSampleEntity();

        protected abstract TKey1 GetExistingKey1();
        protected abstract TKey2 GetExistingKey2();

        protected abstract TKey1 GetNonExistingKey1();
        protected abstract TKey2 GetNonExistingKey2();

        protected abstract TCreateDto GetValidCreateDto();

        protected abstract TController CreateController(Mock<IJoinRepository<TEntity, TKey1, TKey2>> mockRepo, AutoMapper.IMapper mapper);

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            _mockRepository = new Mock<IJoinRepository<TEntity, TKey1, TKey2>>();
            _controller = CreateController(_mockRepository, _mapper);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WithListOfDtos()
        {
            // Given
            List<TEntity> entities = GetSampleEntities();
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(entities);

            // When
            ActionResult<IEnumerable<TDto>> actionResult = await _controller.GetAll();

            // Then
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once, "GetAllAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result type should be OkObjectResult (200).");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            List<TDto> actualValues = (okResult.Value as IEnumerable<TDto>)?.ToList();
            List<TDto> expectedValues = _mapper.Map<IEnumerable<TDto>>(entities).ToList();

            Assert.IsNotNull(actualValues, "Returned value should not be null.");
            CollectionAssert.AreEqual(expectedValues, actualValues, "The returned lists are not identical.");
        }

        [TestMethod]
        public async Task GetById_ExistingIds_ShouldReturnOk_WithDto()
        {
            // Given
            TKey1 key1 = GetExistingKey1();
            TKey2 key2 = GetExistingKey2();
            TEntity entity = GetSampleEntity();

            _mockRepository.Setup(repo => repo.GetByIdAsync(key1, key2)).ReturnsAsync(entity);

            // When
            ActionResult<TDto> actionResult = await _controller.GetById(key1, key2);

            // Then
            _mockRepository.Verify(repo => repo.GetByIdAsync(key1, key2), Times.Once, "GetByIdAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result type should be OkObjectResult (200).");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            TDto actualDto = okResult.Value as TDto;
            TDto expectedDto = _mapper.Map<TDto>(entity);

            Assert.IsNotNull(actualDto, "Returned value should not be null.");
            Assert.AreEqual(expectedDto, actualDto, "The returned DTO does not match.");
        }

        [TestMethod]
        public async Task GetById_NonExistingIds_ShouldReturnNotFound()
        {
            // Given
            TKey1 key1 = GetNonExistingKey1();
            TKey2 key2 = GetNonExistingKey2();

            _mockRepository.Setup(repo => repo.GetByIdAsync(key1, key2)).ReturnsAsync((TEntity)null);

            // When
            ActionResult<TDto> actionResult = await _controller.GetById(key1, key2);

            // Then
            _mockRepository.Verify(repo => repo.GetByIdAsync(key1, key2), Times.Once, "GetByIdAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult), "Result type should be NotFoundResult (404).");
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            TCreateDto createDto = GetValidCreateDto();
            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<TEntity>())).Returns(Task.CompletedTask);

            // When
            ActionResult actionResult = await _controller.Post(createDto);

            // Then
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<TEntity>()), Times.Once, "AddAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(OkResult), "Result type should be OkResult (200).");
        }

        [TestMethod]
        public async Task Post_InvalidObject_ShouldReturnBadRequest()
        {
            // Given
            _controller.ModelState.AddModelError("Error", "Force Invalid State");
            TCreateDto createDto = GetValidCreateDto();

            // When
            ActionResult actionResult = await _controller.Post(createDto);

            // Then
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<TEntity>()), Times.Never, "AddAsync should NOT have been called.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult), "Result type should be BadRequestObjectResult (400).");
        }

        [TestMethod]
        public async Task Delete_ExistingIds_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            TKey1 key1 = GetExistingKey1();
            TKey2 key2 = GetExistingKey2();
            TEntity entity = GetSampleEntity();

            _mockRepository.Setup(repo => repo.GetByIdAsync(key1, key2)).ReturnsAsync(entity);
            _mockRepository.Setup(repo => repo.DeleteAsync(entity)).Returns(Task.CompletedTask);

            // When
            IActionResult actionResult = await _controller.Delete(key1, key2);

            // Then
            _mockRepository.Verify(repo => repo.GetByIdAsync(key1, key2), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteAsync(entity), Times.Once, "DeleteAsync should have been called.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Result type should be NoContentResult (204).");
        }

        [TestMethod]
        public async Task Delete_NonExistingIds_ShouldReturnNotFound()
        {
            // Given
            TKey1 key1 = GetNonExistingKey1();
            TKey2 key2 = GetNonExistingKey2();

            _mockRepository.Setup(repo => repo.GetByIdAsync(key1, key2)).ReturnsAsync((TEntity)null);

            // When
            IActionResult actionResult = await _controller.Delete(key1, key2);

            // Then
            _mockRepository.Verify(repo => repo.GetByIdAsync(key1, key2), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TEntity>()), Times.Never, "DeleteAsync should NOT be called.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult), "Result type should be NotFoundResult (404).");
        }
    }
}