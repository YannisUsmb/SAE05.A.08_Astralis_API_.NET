using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Astralis_API.Tests.Controllers
{
    public abstract class ReadableControllerTests<TEntity, TController, TGetAllDto, TGetDto, TId>
        : BaseControllerTests<TEntity, TController>
        where TEntity : class, new()
        where TController : class
        where TGetAllDto : class
        where TGetDto : class
    {
        protected abstract TId GetEntityId(TEntity entity);
        protected abstract TId GetNonExistentId(); // Abstract method to define a fake ID

        protected abstract Task<ActionResult<IEnumerable<TGetAllDto>>> ActionGetAll();
        protected abstract Task<ActionResult<TGetDto>> ActionGetById(TId id);

        [TestMethod]
        public async Task Readable_GetAll_ShouldReturnOk()
        {
            // Given : Existing entities in the database (handled by BaseInitialize)

            // When : Calling the GetAll action
            var result = await ActionGetAll();

            // Then : It returns OkObjectResult with the correct count
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Result is not OkObjectResult.");

            var items = okResult.Value as IEnumerable<TGetAllDto>;
            Assert.IsNotNull(items, "Content is null.");
            Assert.AreEqual(GetSampleEntities().Count, items.Count(), "Count mismatch.");
        }

        [TestMethod]
        public async Task Readable_GetById_ShouldReturnItem()
        {
            // Given : An existing entity
            var entity = _context.Set<TEntity>().First();
            var id = GetEntityId(entity);

            // When : Calling GetById with the entity's ID
            var result = await ActionGetById(id);

            // Then : It returns OkObjectResult with the correct DTO type
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Result is not OkObjectResult.");
            Assert.IsInstanceOfType(okResult.Value, typeof(TGetDto));
        }

        [TestMethod]
        public async Task Readable_GetById_ShouldReturnNotFound()
        {
            // Given : A non-existent ID
            var id = GetNonExistentId();

            // When : Calling GetById with this ID
            var result = await ActionGetById(id);

            // Then : It returns NotFoundResult
            Assert.IsNull(result.Value, "Result value should be null.");
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult), "Result is not NotFoundResult.");
        }
    }
}