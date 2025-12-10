using Astralis_API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Astralis_APITests.Controllers.Mock
{
    [TestClass]
    public abstract class JoinControllerTests<TController, TEntity, TDto, TCreateDto, TKey1, TKey2>
        : BaseControllerTests<TEntity, TController>
        where TEntity : class, new()
        where TController : JoinController<TEntity, TDto, TCreateDto, TKey1, TKey2>
        where TDto : class
        where TCreateDto : class
        where TKey1 : notnull
        where TKey2 : notnull
    {
        protected abstract TKey1 GetKey1(TEntity entity);
        protected abstract TKey2 GetKey2(TEntity entity);

        protected abstract TKey1 GetNonExistingKey1();
        protected abstract TKey2 GetNonExistingKey2();

        protected abstract TCreateDto GetValidCreateDto();

        protected abstract (TKey1, TKey2) GetKeysFromCreateDto(TCreateDto dto);

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_AndIncludeSeededItems()
        {
            // Given
            List<TEntity> seededEntities = _context.Set<TEntity>().Local.ToList();
            List<TDto> expectedDtos = _mapper.Map<List<TDto>>(seededEntities);

            // When
            ActionResult<IEnumerable<TDto>> actionResult = await _controller.GetAll();

            // Then
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result is not an OkObjectResult (200).");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            IEnumerable<TDto>? actualValuesEnumerable = okResult.Value as IEnumerable<TDto>;
            Assert.IsNotNull(actualValuesEnumerable, "Returned list is null.");

            List<TDto> actualValues = actualValuesEnumerable.ToList();

            CollectionAssert.IsSubsetOf(expectedDtos, actualValues, "Some seeded items were not found in the returned list.");
        }

        [TestMethod]
        public async Task GetById_ExistingIds_ShouldReturnOk_AndCorrectItem()
        {
            // Given
            TEntity entityToFind = _seededEntities.First();
            TKey1 key1 = GetKey1(entityToFind);
            TKey2 key2 = GetKey2(entityToFind);

            TDto expectedDto = _mapper.Map<TDto>(entityToFind);

            // When
            ActionResult<TDto> actionResult = await _controller.GetById(key1, key2);

            // Then
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result is not 200 OK.");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            TDto? actualDto = okResult.Value as TDto;

            Assert.IsNotNull(actualDto, "Returned DTO is null.");
            Assert.AreEqual(expectedDto, actualDto, "The returned DTO does not match the seeded entity.");
        }

        [TestMethod]
        public async Task GetById_NonExistingIds_ShouldReturnNotFound()
        {
            // Given
            TKey1 key1 = GetNonExistingKey1();
            TKey2 key2 = GetNonExistingKey2();

            // When
            ActionResult<TDto> actionResult = await _controller.GetById(key1, key2);

            // Then
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult), "Result should be 404 Not Found.");
        }

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            TCreateDto createDto = GetValidCreateDto();
            (TKey1 key1, TKey2 key2) = GetKeysFromCreateDto(createDto);

            // When
            ActionResult actionResult = await _controller.Post(createDto);

            // Then
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(OkResult), "Result is not an OkResult (200).");

            TEntity? createdEntity = await _context.Set<TEntity>().FindAsync(key1, key2);

            Assert.IsNotNull(createdEntity, "The created join entity was not found in the database.");
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
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult), "Result is not a BadRequestObjectResult (400).");
        }

        [TestMethod]
        public async Task Delete_ExistingIds_ShouldDeleteAndReturnNoContent()
        {
            // Given
            TEntity entityToDelete = _seededEntities.First();
            TKey1 key1 = GetKey1(entityToDelete);
            TKey2 key2 = GetKey2(entityToDelete);

            // When
            IActionResult actionResult = await _controller.Delete(key1, key2);

            // Then
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Result is not a NoContentResult (204).");

            _context.Entry(entityToDelete).State = EntityState.Detached;
            TEntity? deletedEntity = await _context.Set<TEntity>().FindAsync(key1, key2);

            Assert.IsNull(deletedEntity, "The entity should have been deleted from the database.");
        }

        [TestMethod]
        public async Task Delete_NonExistingIds_ShouldReturnNotFound()
        {
            // Given
            TKey1 key1 = GetNonExistingKey1();
            TKey2 key2 = GetNonExistingKey2();

            // When
            IActionResult actionResult = await _controller.Delete(key1, key2);

            // Then
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult), "Result is not a NotFoundResult (404).");
        }
    }
}