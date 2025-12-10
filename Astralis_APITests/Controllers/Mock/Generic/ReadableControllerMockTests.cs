using Astralis_API.Controllers;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;

namespace Astralis_APITests.Controllers
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

        protected abstract TController CreateController(IReadableRepository<TEntity, TId> repository, AutoMapper.IMapper mapper);

        [TestInitialize]
        public override void BaseInitialize()
        {
            base.BaseInitialize();
            _mockRepository = new Mock<IReadableRepository<TEntity, TId>>();
            _controller = CreateController(_mockRepository.Object, _mapper);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WithListOfDtos()
        {
            // Arrange
            List<TEntity> entities = GetSampleEntities();
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(entities);

            // Act
            ActionResult<IEnumerable<TGetAllDto>> actionResult = await _controller.GetAll();

            // Assert
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once, "GetAllAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<TGetAllDto>>), "Pas un ActionResult<IEnumerable<BrandDTO>>.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result type should be OkObjectResult (200).");

            // 3. Verify Data Mapping
            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            List<TGetAllDto> actualValues = (okResult.Value as IEnumerable<TGetAllDto>)?.ToList();

            Assert.IsNotNull(actualValues, "Returned value should not be null.");
            Assert.AreEqual(entities.Count, actualValues.Count, "The number of returned elements does not match the mock data.");
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOk_WithDto()
        {
            // Arrange
            TId id = GetExistingId();
            TEntity entity = GetSampleEntity();
            _mockRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(entity);

            // Act
            ActionResult<TGetDto> actionResult = await _controller.GetById(id);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once, "GetByIdAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result type should be OkObjectResult (200).");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(TGetDto), "Returned value is not of the expected DTO type.");
        }

        [TestMethod]
        public async Task GetById_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            TId id = GetNonExistingId();
            _mockRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((TEntity)null);

            // Act
            ActionResult<TGetDto> actionResult = await _controller.GetById(id);

            // Assert
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once, "GetByIdAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult), "Result type should be NotFoundResult (404).");
        }
    }
}