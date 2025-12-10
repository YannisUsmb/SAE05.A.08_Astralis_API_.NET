using Astralis_API.Controllers;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public abstract class ReadableControllerMockTests<TController, TEntity, TGetAllDto, TGetDto, TId> : BaseControllerMockTests
        where TEntity : class
        where TController : ReadableController<TEntity, TGetAllDto, TGetDto, TId>
        where TGetAllDto : class
        where TGetDto : class
    {
        protected Mock<IReadableRepository<TEntity, TId>> _mockRepository;
        protected TController _controller;

        protected abstract List<TEntity> GetSampleEntities();
        protected abstract TEntity GetSampleEntity();
        protected abstract TId GetExistingId();
        protected abstract TId GetNonExistingId();

        protected abstract TController CreateController(Mock<IReadableRepository<TEntity, TId>> mockRepo, AutoMapper.IMapper mapper);

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            _mockRepository = new Mock<IReadableRepository<TEntity, TId>>();

            _controller = CreateController(_mockRepository, _mapper);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WithListOfDtos()
        {
            // Given
            List<TEntity> entities = GetSampleEntities();
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(entities);

            // When
            ActionResult<IEnumerable<TGetAllDto>> actionResult = await _controller.GetAll();

            // Then
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once, "GetAllAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<TGetAllDto>>), $"Not an ActionResult<IEnumerable<{typeof(TGetAllDto).Name}>>.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result type should be OkObjectResult (200).");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            List<TGetAllDto> expectedValues = _mapper.Map<IEnumerable<TGetAllDto>>(entities).ToList();
            List<TGetAllDto> actualValues = (okResult.Value as IEnumerable<TGetAllDto>)?.ToList();

            Assert.IsNotNull(actualValues, "Returned value should not be null.");

            CollectionAssert.AreEqual(expectedValues, actualValues, "The returned lists are not identical.");
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOk_WithDto()
        {
            // Given
            TId id = GetExistingId();
            TEntity entity = GetSampleEntity();

            _mockRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(entity);

            // When
            ActionResult<TGetDto> actionResult = await _controller.GetById(id);

            // Then
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once, "GetByIdAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<TGetDto>), $"Not an ActionResult<{typeof(TGetDto).Name}>.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result type should be OkObjectResult (200).");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            TGetDto expectedDto = _mapper.Map<TGetDto>(entity);
            TGetDto actualDto = okResult.Value as TGetDto;

            Assert.IsNotNull(actualDto, "Returned value should not be null.");
            Assert.AreEqual(expectedDto, actualDto, "The returned DTO does not match the expected one.");
        }

        [TestMethod]
        public async Task GetById_NonExistingId_ShouldReturnNotFound()
        {
            // Given
            TId id = GetNonExistingId();
            _mockRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((TEntity)null);

            // When
            ActionResult<TGetDto> actionResult = await _controller.GetById(id);

            // Then
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once, "GetByIdAsync should have been called once.");

            Assert.IsNull(actionResult.Value, "The result value should be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult), "Result type should be NotFoundResult (404).");
        }
    }
}