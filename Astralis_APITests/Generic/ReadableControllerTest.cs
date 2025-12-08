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

        protected abstract Task<ActionResult<IEnumerable<TGetAllDto>>> ActionGetAll();
        protected abstract Task<ActionResult<TGetDto>> ActionGetById(TId id);

        [TestMethod]
        public async Task Readable_GetAll_ShouldReturnOk()
        {
            var result = await ActionGetAll();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Le résultat n'est pas un OkObjectResult (200).");

            var items = okResult.Value as IEnumerable<TGetAllDto>;
            Assert.IsNotNull(items, "Le contenu est null.");
            Assert.AreEqual(GetSampleEntities().Count, items.Count(), "Le nombre d'éléments retournés est incorrect.");
        }

        [TestMethod]
        public async Task Readable_GetById_ShouldReturnItem()
        {
            var entity = _context.Set<TEntity>().First();
            var id = GetEntityId(entity);

            var result = await ActionGetById(id);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Le résultat n'est pas un OkObjectResult (200).");
            Assert.IsInstanceOfType(okResult.Value, typeof(TGetDto));
        }
    }
}