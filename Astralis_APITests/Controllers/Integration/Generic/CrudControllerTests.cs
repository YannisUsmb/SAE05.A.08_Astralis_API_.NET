using Astralis_API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Astralis_APITests.Controllers
{
    [TestClass]
    public abstract class CrudControllerTests<TController, TEntity, TGetAllDto, TGetDto, TCreateDto, TUpdateDto, TId>
        : ReadableControllerTests<TController, TEntity, TGetAllDto, TGetDto, TId>
        where TEntity : class, new()
        where TController : CrudController<TEntity, TGetAllDto, TGetDto, TCreateDto, TUpdateDto, TId>
        where TGetAllDto : class
        where TGetDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected abstract TCreateDto GetValidCreateDto();
        protected abstract TUpdateDto GetValidUpdateDto(TEntity entityToUpdate);
        protected abstract TId GetIdFromDto(TGetDto dto);

        protected abstract void SetIdInUpdateDto(TUpdateDto dto, TId id);

        [TestMethod]
        public async Task Post_ValidObject_ShouldCreateAndReturn200()
        {
            // Arrange
            TCreateDto createDto = GetValidCreateDto();

            // Act
            ActionResult<TGetDto> actionResult = await _controller.Post(createDto);

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<TGetDto>), $"Not an ActionResult<{typeof(TGetDto).Name}>.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result is not an OkObjectResult (200).");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            TGetDto? actualDto = okResult.Value as TGetDto;
            Assert.IsNotNull(actualDto, "Returned DTO should not be null.");

            TId createdId = GetIdFromDto(actualDto);
            TEntity? createdEntity = await _context.Set<TEntity>().FindAsync(createdId);
            Assert.IsNotNull(createdEntity, "The created entity was not found in the database.");

            TGetDto expectedDtoFromDb = _mapper.Map<TGetDto>(createdEntity);
            Assert.AreEqual(expectedDtoFromDb, actualDto, "The returned DTO does not match the entity in database.");
        }

        [TestMethod]
        public async Task Post_InvalidObject_ShouldReturn400()
        {
            // Arrange
            _controller.ModelState.AddModelError("Key", "Invalid Error");
            TCreateDto createDto = GetValidCreateDto();

            // Act
            ActionResult<TGetDto> actionResult = await _controller.Post(createDto);

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult), "Result is not a BadRequestObjectResult (400).");
        }

        [TestMethod]
        public async Task Put_ValidObject_ShouldUpdateAndReturn204()
        {
            // Arrange
            TEntity entityToUpdate = _seededEntities.First();
            TId id = GetIdFromEntity(entityToUpdate);
            TUpdateDto updateDto = GetValidUpdateDto(entityToUpdate);

            SetIdInUpdateDto(updateDto, id);

            // Act
            IActionResult actionResult = await _controller.Put(id, updateDto);

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Result is not a NoContentResult (204).");
        }

        //[TestMethod]
        //public async Task Put_IdMismatch_ShouldReturn400()
        //{
        //    // Arrange
        //    TEntity entityToUpdate = _seededEntities.First();
        //    TId validId = GetIdFromEntity(entityToUpdate);

        //    TUpdateDto updateDto = GetValidUpdateDto(entityToUpdate);
        //    TId otherId = GetNonExistingId();
        //    SetIdInUpdateDto(updateDto, otherId);

        //    // Act
        //    IActionResult actionResult = await _controller.Put(validId, updateDto);

        //    // Assert
        //    Assert.IsNotNull(actionResult, "Result should not be null.");

        //    Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult), "Result is not a BadRequestObjectResult (400) for ID mismatch.");
        //}

        [TestMethod]
        public async Task Put_NonExistingId_ShouldReturn404()
        {
            // Arrange
            TUpdateDto updateDto = GetValidUpdateDto(_seededEntities.First());
            TId invalidId = GetNonExistingId();
            SetIdInUpdateDto(updateDto, invalidId);

            // Act
            IActionResult actionResult = await _controller.Put(invalidId, updateDto);

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult), "Result is not a NotFoundResult (404).");
        }

        [TestMethod]
        public async Task Delete_ExistingId_ShouldDeleteAndReturn204()
        {
            // Arrange
            TEntity entityToDelete = _seededEntities.First();
            TId id = GetIdFromEntity(entityToDelete);

            // Act
            IActionResult actionResult = await _controller.Delete(id);

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Result is not a NoContentResult (204).");

            _context.Entry(entityToDelete).State = EntityState.Detached;
            TEntity? deletedEntity = await _context.Set<TEntity>().FindAsync(id);

            Assert.IsNull(deletedEntity, "The entity should have been deleted from the database.");
        }

        [TestMethod]
        public async Task Delete_NonExistingId_ShouldReturn404()
        {
            // Arrange
            TId invalidId = GetNonExistingId();

            // Act
            IActionResult actionResult = await _controller.Delete(invalidId);

            // Assert
            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult), "Result is not a NotFoundResult (404).");
        }
    }
}