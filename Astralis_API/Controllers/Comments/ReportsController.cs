using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Services.Interfaces;
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
    [DisplayName("Report")]
    public class ReportsController : CrudController<Report, ReportDto, ReportDto, ReportCreateDto, ReportUpdateDto, int>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IEmailService _emailService;

        public ReportsController(IReportRepository repository, IEmailService emailService, IMapper mapper)
            : base(repository, mapper)
        {
            _reportRepository = repository;
            _emailService = emailService;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<ReportDto>> Post(ReportCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var report = _mapper.Map<Report>(createDto);
            report.UserId = userId;
            report.Date = DateTime.UtcNow;
            report.ReportStatusId = 1;

            await _repository.AddAsync(report);

            try
            {
                string? userEmail = User.FindFirstValue(ClaimTypes.Email);
                string? username = User.FindFirstValue(ClaimTypes.Name);

                if (!string.IsNullOrEmpty(userEmail))
                {
                    string subject = "Confirmation de votre signalement - Astralis";
                    string message = $@"
                        <div style='background: #0f0c29; color: #e0e0e0; padding: 30px; font-family: sans-serif; border-radius: 10px;'>
                            <h2 style='color: #a76dff; margin-bottom: 20px;'>Signalement reçu</h2>
                            <p>Bonjour <strong>{username ?? "Explorateur"}</strong>,</p>
                            <p>Nous vous confirmons la bonne réception de votre signalement concernant un commentaire.</p>
                            <div style='background: rgba(255, 255, 255, 0.05); padding: 15px; border-left: 4px solid #a76dff; margin: 20px 0; font-style: italic; color: #ccc;'>
                                ""{createDto.Description ?? "Aucune description fournie"}""
                            </div>
                            <p>Nos équipes de modération vont analyser la situation dans les plus brefs délais et prendront les mesures nécessaires conformément à nos règles communautaires.</p>
                            <p>Merci de contribuer à la sécurité et à la qualité des échanges sur Astralis.</p>
                            <hr style='border: 0; border-top: 1px solid rgba(255, 255, 255, 0.1); margin: 30px 0;'>
                            <p style='font-size: 12px; color: #888;'>Ceci est un message automatique, merci de ne pas y répondre.</p>
                        </div>";

                    await _emailService.SendEmailAsync(userEmail, subject, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi de l'email de signalement : {ex.Message}");
            }

            var resultDto = _mapper.Map<ReportDto>(report);
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, resultDto);
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

        /// <summary>
        /// Searches for reports based on status, motive, or date range.
        /// </summary>
        /// <param name="filter">Filter criteria.</param>
        /// <returns>A list of matching reports.</returns>
        /// <response code="200">List of reports retrieved successfully.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">User not authorized (Admins only).</response>
        [HttpGet("Search")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ReportDto>>> Search([FromQuery] ReportFilterDto filter)
        {
            IEnumerable<Report?> reports = await _reportRepository.SearchAsync(
                filter.StatusId,
                filter.MotiveId,
                filter.MinDate,
                filter.MaxDate
            );

            return Ok(_mapper.Map<IEnumerable<ReportDto>>(reports));
        }
    }
}