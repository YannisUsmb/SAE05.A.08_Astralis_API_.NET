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
        protected abstract TId GetNonExistentId();

        // --- NOUVEAU : On doit savoir comment récupérer l'ID depuis le DTO ---
        protected abstract TId GetDtoId(TGetAllDto dto);

        protected abstract Task<ActionResult<IEnumerable<TGetAllDto>>> ActionGetAll();
        protected abstract Task<ActionResult<TGetDto>> ActionGetById(TId id);

        [TestMethod]
        public async Task Readable_GetAll_ShouldReturnOk()
        {
            var result = await ActionGetAll();
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Le résultat n'est pas un OkObjectResult (200).");
            var actualAllDtos = okResult.Value as IEnumerable<TGetAllDto>;
            Assert.IsNotNull(actualAllDtos, "Le contenu est null.");
            var expectedEntities = GetSampleEntities();
            var expectedDtos = _mapper.Map<IEnumerable<TGetAllDto>>(expectedEntities).ToList();
            foreach (var expectedDto in expectedDtos)
            {
                var expectedId = GetDtoId(expectedDto);

                var actualDto = actualAllDtos.FirstOrDefault(d => GetDtoId(d).Equals(expectedId));

                Assert.IsNotNull(actualDto, $"L'élément avec l'ID {expectedId} inséré pour le test n'a pas été retrouvé dans le GetAll.");
            }

            // Vérification du compte : On doit avoir retrouvé autant d'éléments qu'insérés
            var countFound = actualAllDtos.Count(d => expectedDtos.Any(e => GetDtoId(e).Equals(GetDtoId(d))));
            Assert.AreEqual(expectedDtos.Count, countFound, "Tous les éléments insérés n'ont pas été retrouvés.");
        }

        [TestMethod]
        public async Task Readable_GetById_ShouldReturnItem()
        {
            var entity = _context.Set<TEntity>().First();
            var id = GetEntityId(entity);
            var result = await ActionGetById(id);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(TGetDto));
        }

        [TestMethod]
        public async Task Readable_GetById_ShouldReturnNotFound()
        {
            var id = GetNonExistentId();
            var result = await ActionGetById(id);
            Assert.IsNull(result.Value);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }        
    }
}