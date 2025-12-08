using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Astralis_API.Tests.Controllers
{
    public abstract class CrudControllerTests<TEntity, TController, TGetAllDto, TGetDto, TCreateDto, TUpdateDto, TId>
        : ReadableControllerTests<TEntity, TController, TGetAllDto, TGetDto, TId>
        where TEntity : class, new()
        where TController : class
        where TGetAllDto : class
        where TGetDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected abstract void UpdateEntityForTest(TEntity entity);

        protected abstract Task<ActionResult<TGetDto>> ActionPost(TCreateDto dto);
        protected abstract Task<IActionResult> ActionPut(TId id, TUpdateDto dto);
        protected abstract Task<IActionResult> ActionDelete(TId id);

        [TestMethod]
        public async Task Crud_Post_ShouldCreateItem()
        {
            var newItem = new TEntity();
            UpdateEntityForTest(newItem);
            var createDto = _mapper.Map<TCreateDto>(newItem);

            var result = await ActionPost(createDto);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Le résultat n'est pas un OkObjectResult (200).");
            Assert.IsInstanceOfType(okResult.Value, typeof(TGetDto));
        }

        [TestMethod]
        public async Task Crud_Put_ShouldUpdateItem()
        {
            var entity = _context.Set<TEntity>().First();
            UpdateEntityForTest(entity);
            var updateDto = _mapper.Map<TUpdateDto>(entity);
            var id = GetEntityId(entity);

            var result = await ActionPut(id, updateDto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult), "Le résultat n'est pas NoContent (204).");
        }

        [TestMethod]
        public async Task Crud_Delete_ShouldRemoveItem()
        {
            var entity = _context.Set<TEntity>().First();
            var id = GetEntityId(entity);

            var result = await ActionDelete(id);

            Assert.IsInstanceOfType(result, typeof(NoContentResult), "Le résultat n'est pas NoContent (204).");

            var deletedEntity = await _context.Set<TEntity>().FindAsync(id);
            Assert.IsNull(deletedEntity, "L'entité devrait être supprimée de la BDD.");
        }
    }
}