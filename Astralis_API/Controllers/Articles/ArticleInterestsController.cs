using Astralis.Shared.DTOs;
using Astralis_API.Configuration;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("Like")]
    public class ArticleInterestsController : JoinController<ArticleInterest, ArticleInterestDto, int, int>
    {
        public ArticleInterestsController(IArticleInterestRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        /// <summary>
        /// Adds a like to an article.
        /// </summary>
        /// <param name="dto">Contains ArticleId and UserId.</param>
        /// <response code="200">Like added successfully.</response>
        /// <response code="400">Invalid user ID (must match authenticated user).</response>
        /// <response code="401">User not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public override async Task<ActionResult> Post(ArticleInterestDto dto)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || dto.UserId != userId)
            {
                return BadRequest("You can only add likes for your own account.");
            }
            return await base.Post(dto);
        }

        /// <summary>
        /// Removes a like from an article.
        /// </summary>
        /// <param name="id1">Article ID.</param>
        /// <param name="id2">User ID.</param>
        /// <response code="204">Like removed successfully.</response>
        /// <response code="400">Invalid user ID.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="404">Like not found.</response>
        [HttpDelete("{id1}/{id2}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> Delete(int id1, int id2)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || id2 != userId)
            {
                return BadRequest("You can only remove your own likes.");
            }
            return await base.Delete(id1, id2);
        }
    }
}