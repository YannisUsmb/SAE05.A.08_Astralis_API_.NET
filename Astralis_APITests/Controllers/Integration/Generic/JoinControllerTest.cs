using Microsoft.AspNetCore.Mvc;

namespace Astralis_APITests.Controllers
{
    public abstract class JoinControllerTests<TEntity, TController, TDto, TCreateDto, TKey1, TKey2>
        : BaseControllerTests<TEntity, TController>
        where TEntity : class, new()
        where TController : class
        where TDto : class
        where TCreateDto : class
    {
        protected abstract (TKey1, TKey2) GetEntityKeys(TEntity entity);
        protected abstract Task<ActionResult<IEnumerable<TDto>>> ActionGetAll();
        protected abstract Task<ActionResult<TDto>> ActionGetById(TKey1 id1, TKey2 id2);
        protected abstract Task<ActionResult> ActionPost(TCreateDto dto);
        protected abstract Task<IActionResult> ActionDelete(TKey1 id1, TKey2 id2);

        [TestMethod]
        public async Task Join_GetAll_ShouldReturnList()
        {
            var result = await ActionGetAll();
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var items = okResult.Value as IEnumerable<TDto>;
            Assert.AreEqual(GetSampleEntities().Count, items.Count());
        }

        [TestMethod]
        public async Task Join_GetById_ShouldReturnItem()
        {
            var entity = _context.Set<TEntity>().First();
            var (id1, id2) = GetEntityKeys(entity);

            var result = await ActionGetById(id1, id2);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(TDto));
        }

        [TestMethod]
        public async Task Join_Post_ShouldCreateAssociation()
        {
            var newItem = new TEntity();
            var createDto = _mapper.Map<TCreateDto>(newItem);

            var result = await ActionPost(createDto);

            var okResult = result as OkResult;
            Assert.IsNotNull(okResult, "Le résultat n'est pas Ok (200).");
        }

        [TestMethod]
        public async Task Join_Delete_ShouldRemoveAssociation()
        {
            var entity = _context.Set<TEntity>().First();
            var (id1, id2) = GetEntityKeys(entity);

            var result = await ActionDelete(id1, id2);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var deleted = await _context.Set<TEntity>().FindAsync(id1, id2);
            Assert.IsNull(deleted);
        }
    }
}