using Astralis_API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public abstract class ReadableControllerTests<TController, TEntity, TGetAllDto, TGetDto, TId>
        : BaseControllerTests<TEntity, TController>
        where TEntity : class, new()
        where TController : ReadableController<TEntity, TGetAllDto, TGetDto, TId>
        where TGetAllDto : class
        where TGetDto : class
    {
        protected abstract TId GetNonExistingId();

        [TestMethod]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            List<TGetAllDto> expectedValues = _mapper.Map<List<TGetAllDto>>(_seededEntities);

            // Act
            ActionResult<IEnumerable<TGetAllDto>> actionResult = await _controller.GetAll();

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");

            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result is not an OkObjectResult (200).");
            OkObjectResult okResult = (OkObjectResult)actionResult.Result;

            IEnumerable<TGetAllDto>? actualValues = okResult.Value as IEnumerable<TGetAllDto>;
            Assert.IsNotNull(actualValues, "Returned list is null.");

            List<TGetAllDto> actualValuesList = actualValues.ToList();

            CollectionAssert.IsSubsetOf(expectedValues, actualValuesList, "Some seeded items were not found in the returned list.");
        }

        [TestMethod]
        public async Task GetById_ExistingId_ShouldReturnOkAndCorrectItem()
        {
            // Arrange
            TEntity entityToFind = _seededEntities.First();

            TId validId = GetIdFromEntity(entityToFind);

            TGetDto expectedDto = _mapper.Map<TGetDto>(entityToFind);

            // Act
            var actionResult = await _controller.GetById(validId);

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result is not 200 OK.");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            TGetDto? actualDto = okResult.Value as TGetDto;

            Assert.IsNotNull(actualDto, "Returned DTO is null.");

            Assert.AreEqual(expectedDto, actualDto, "The returned DTO does not match the seeded entity.");
        }

        [TestMethod]
        public async Task GetById_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            TId invalidId = GetNonExistingId();

            // Act
            ActionResult<TGetDto>? actionResult = await _controller.GetById(invalidId);

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult), "Result should be 404 Not Found.");
            Assert.IsNull(actionResult.Value, "Value should be null for NotFound result.");
        }

        protected abstract TId GetIdFromEntity(TEntity entity);
    }
}