using Astralis.Shared.DTOs;
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
    [Authorize(Roles = "Admin")]
    [DisplayName("Report")]
    public class ReportsController : CrudController<Report, ReportDto, ReportDto, ReportCreateDto, ReportUpdateDto, int>
    {
        public ReportsController(IReportRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        /// <summary>
        /// Creates a new report for a comment.
        /// </summary>
        /// <param name="createDto">The report details.</param>
        /// <returns>The created report.</returns>
        /// <response code="200">Report successfully created.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="401">User not authenticated.</response>
        [HttpPost]
        [Authorize(Roles = "Client")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<ReportDto>> Post(ReportCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Impossible user authentification.");
            }

            Report entity = _mapper.Map<Report>(createDto);

            entity.UserId = userId;
            entity.Date = DateTime.UtcNow;
            entity.ReportStatusId = 1;
            entity.AdminId = null;

            await _repository.AddAsync(entity);

            var returnDto = _mapper.Map<ReportDto>(entity);
            return Ok(returnDto);
        }

        /// <summary>
        /// Updates a report status (Admin only).
        /// </summary>
        /// <param name="id">Report ID.</param>
        /// <param name="updateDto">New status.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The report was successfully updated.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User not authorized (requires Admin role).</response>
        /// <response code="404">Report not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> Put(int id, ReportUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("Url ID doesn't match the request body.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Report? entityToUpdate = await _repository.GetByIdAsync(id);
            if (entityToUpdate == null)
                return NotFound();

            string? adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(adminIdString, out int adminId))
            {
                entityToUpdate.AdminId = adminId;
            }

            _mapper.Map(updateDto, entityToUpdate);

            await _repository.UpdateAsync(entityToUpdate, entityToUpdate);

            return NoContent();
        }
    }
}