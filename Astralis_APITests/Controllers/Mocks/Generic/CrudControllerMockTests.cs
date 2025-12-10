using Astralis_API.Controllers;
using Astralis_API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Astralis_APITests.Controllers.Mocks
{
    [TestClass]
    public abstract class CrudControllerMockTests<TController, TEntity, TGetAllDto, TGetDto, TCreateDto, TUpdateDto, TId>
        : ReadableControllerMockTests<TController, TEntity, TGetAllDto, TGetDto, TId>
        where TEntity : class
        where TController : CrudController<TEntity, TGetAllDto, TGetDto, TCreateDto, TUpdateDto, TId>
        where TGetAllDto : class
        where TGetDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected Mock<ICrudRepository<TEntity, TId>> _mockCrudRepository;

        protected abstract TCreateDto GetValidCreateDto();
        protected abstract TUpdateDto GetValidUpdateDto();

        protected abstract void SetIdInUpdateDto(TUpdateDto dto, TId id);

        [TestMethod]
        public async Task Post_ValidObject_ShouldCallAddAndReturnOk()
        {
            // Given
            TCreateDto createDto = GetValidCreateDto();

            _mockCrudRepository.Setup(repo => repo.AddAsync(It.IsAny<TEntity>())).Returns(Task.CompletedTask);

            // When
            ActionResult<TGetDto> actionResult = await _controller.Post(createDto);

            // Then
            _mockCrudRepository.Verify(repo => repo.AddAsync(It.IsAny<TEntity>()), Times.Once, "AddAsync should have been called once.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult), "Result type should be OkObjectResult (200).");

            OkObjectResult okResult = (OkObjectResult)actionResult.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(TGetDto), "Returned value is not of the expected DTO type.");
        }

        [TestMethod]
        public async Task Post_InvalidObject_ShouldReturnBadRequest()
        {
            // Given
            _controller.ModelState.AddModelError("Error", "Sample error to simulate invalid state");
            TCreateDto createDto = GetValidCreateDto();

            // When
            ActionResult<TGetDto> actionResult = await _controller.Post(createDto);

            // Then
            _mockCrudRepository.Verify(repo => repo.AddAsync(It.IsAny<TEntity>()), Times.Never, "AddAsync should NOT have been called when model state is invalid.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult), "Result type should be BadRequestObjectResult (400).");
        }

        [TestMethod]
        public async Task Put_ValidObject_ShouldCallUpdateAndReturnNoContent()
        {
            // Given
            TId id = GetExistingId();
            TEntity existingEntity = GetSampleEntity();
            TUpdateDto updateDto = GetValidUpdateDto();
            SetIdInUpdateDto(updateDto, id);

            _mockCrudRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingEntity);
            _mockCrudRepository.Setup(repo => repo.UpdateAsync(existingEntity, It.IsAny<TEntity>())).Returns(Task.CompletedTask);

            // When
            IActionResult actionResult = await _controller.Put(id, updateDto);

            // Then
            _mockCrudRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once, "GetByIdAsync should serve as check before update.");
            _mockCrudRepository.Verify(repo => repo.UpdateAsync(existingEntity, It.IsAny<TEntity>()), Times.Once, "UpdateAsync should have been called.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Result type should be NoContentResult (204).");
        }

        //[TestMethod]
        //public async Task Put_IdMismatch_ShouldReturnBadRequest()
        //{
        //    // Ce test ne fonctionne que si le contrôleur implémente la vérification (id != dto.Id)

        //    // Arrange
        //    TId id = GetExistingId();
        //    TId otherId = GetNonExistingId();
        //    TUpdateDto updateDto = GetValidUpdateDto();

        //    // On force un ID différent dans le DTO
        //    SetIdInUpdateDto(updateDto, otherId);

        //    // Act
        //    IActionResult actionResult = await _controller.Put(id, updateDto);

        //    // Assert
        //    // Si le DTO ne contient pas d'ID (SetIdInUpdateDto est vide), ce test peut échouer ou passer selon l'implémentation.
        //    // On suppose ici que si le contrôleur vérifie l'ID, il renvoie BadRequest.
        //    if (actionResult is BadRequestObjectResult || actionResult is BadRequestResult)
        //    {
        //        Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult), "Should return BadRequest on ID mismatch.");
        //        _mockCrudRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TEntity>(), It.IsAny<TEntity>()), Times.Never);
        //    }
        //    else
        //    {
        //        // Si le DTO n'a pas d'ID, le contrôleur ne peut pas vérifier le mismatch, donc il continue.
        //        // Ce n'est pas un échec du test, c'est juste non applicable pour ce DTO.
        //    }
        //}

        [TestMethod]
        public async Task Put_NonExistingObject_ShouldReturnNotFound()
        {
            // Given
            TId id = GetNonExistingId();
            TUpdateDto updateDto = GetValidUpdateDto();
            SetIdInUpdateDto(updateDto, id);

            _mockCrudRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((TEntity)null);

            // When
            IActionResult actionResult = await _controller.Put(id, updateDto);

            // Then
            _mockCrudRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _mockCrudRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TEntity>(), It.IsAny<TEntity>()), Times.Never, "UpdateAsync should NOT be called if entity is missing.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult), "Result type should be NotFoundResult (404).");
        }

        [TestMethod]
        public async Task Delete_ValidObject_ShouldCallDeleteAndReturnNoContent()
        {
            // Given
            TId id = GetExistingId();
            TEntity existingEntity = GetSampleEntity();

            _mockCrudRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingEntity);
            _mockCrudRepository.Setup(repo => repo.DeleteAsync(existingEntity)).Returns(Task.CompletedTask);

            // When
            IActionResult actionResult = await _controller.Delete(id);

            // Then
            _mockCrudRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _mockCrudRepository.Verify(repo => repo.DeleteAsync(existingEntity), Times.Once, "DeleteAsync should have been called.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Result type should be NoContentResult (204).");
        }

        [TestMethod]
        public async Task Delete_NonExistingObject_ShouldReturnNotFound()
        {
            // Given
            TId id = GetNonExistingId();

            _mockCrudRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((TEntity)null);

            // When
            IActionResult actionResult = await _controller.Delete(id);

            // Then
            _mockCrudRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _mockCrudRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TEntity>()), Times.Never, "DeleteAsync should NOT be called if entity is missing.");

            Assert.IsNotNull(actionResult, "Result should not be null.");
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult), "Result type should be NotFoundResult (404).");
        }
    }
}