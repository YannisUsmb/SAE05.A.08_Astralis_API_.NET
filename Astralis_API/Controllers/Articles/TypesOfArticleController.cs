using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("Type of Article")]
    public class TypesOfArticleController : JoinController<TypeOfArticle, TypeOfArticleDto, TypeOfArticleDto, int, int>
    {
        public TypesOfArticleController(IJoinRepository<TypeOfArticle, int, int> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        /// <summary>
        /// Retrieves all TypesOfArticle associations.
        /// </summary>
        /// <returns>A list of TypesOfArticle.</returns>
        /// <response code="200">The list of TypesOfArticle was successfully retrieved.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<IEnumerable<TypeOfArticleDto>>> GetAll()
        {
            return await base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific TypeOfArticle by its composite keys.
        /// </summary>
        /// <param name="id1">The first part of the composite key.</param>
        /// <param name="id2">The second part of the composite key.</param>
        /// <returns>The requested TypeOfArticle.</returns>
        /// <response code="200">The TypeOfArticle was successfully retrieved.</response>
        /// <response code="404">The TypeOfArticle does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpGet("{id1}/{id2}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<TypeOfArticleDto>> GetById(int id1, int id2)
        {
            return await base.GetById(id1, id2);
        }

        /// <summary>
        /// Creates a new TypeOfArticle association (Commercial Editor only).
        /// </summary>
        /// <param name="dto">The object containing the details of the TypeOfArticle to create.</param>
        /// <returns>A status 200 OK upon success.</returns>
        /// <response code="200">The TypeOfArticle was successfully created.</response>
        /// <response code="400">The input data is invalid.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpPost]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult> Post(TypeOfArticleDto dto)
        {
            return await base.Post(dto);
        }

        /// <summary>
        /// Deletes a specific TypeOfArticle association (Commercial Editor only).
        /// </summary>
        /// <param name="id1">The first part of the composite key.</param>
        /// <param name="id2">The second part of the composite key.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The TypeOfArticle was successfully deleted.</response>
        /// <response code="404">The TypeOfArticle does not exist.</response>
        /// <response code="500">An internal server error occurred.</response>
        [HttpDelete("{id1}/{id2}")]
        [Authorize(Roles = "Rédacteur commercial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Delete(int id1, int id2)
        {
            return await base.Delete(id1, id2);
        }
    }
}