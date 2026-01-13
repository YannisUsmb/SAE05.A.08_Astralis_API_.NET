using Astralis.Shared.DTOs;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IDiscoveryRepository _discoveryRepository;
        private readonly IMapper _mapper;

        public AdminController(IDiscoveryRepository discoveryRepository, IMapper mapper)
        {
            _discoveryRepository = discoveryRepository;
            _mapper = mapper;
        }

        // GET: api/Admin/Discoveries/Pending
        [HttpGet("Discoveries/Pending")]
        public async Task<ActionResult<IEnumerable<DiscoveryDto>>> GetPendingDiscoveries()
        {
            var pendingDiscoveries = await _discoveryRepository.SearchAsync(discoveryStatusId: 1); 

            return Ok(_mapper.Map<IEnumerable<DiscoveryDto>>(pendingDiscoveries));
        }

        // POST: api/Admin/Discoveries/{id}/Approve
        [HttpPost("Discoveries/{id}/Approve")]
        public async Task<IActionResult> ApproveDiscovery(int id)
        {
            var discovery = await _discoveryRepository.GetByIdAsync(id);
            if (discovery == null) return NotFound();
            
            discovery.DiscoveryStatusId = 3; 
            
            await _discoveryRepository.UpdateAsync(discovery, discovery);
            
            return NoContent();
        }

        // POST: api/Admin/Discoveries/{id}/Reject
        [HttpPost("Discoveries/{id}/Reject")]
        public async Task<IActionResult> RejectDiscovery(int id, [FromBody] DiscoveryRejectionDto rejectionDto)
        {
            var discovery = await _discoveryRepository.GetByIdAsync(id);
            if (discovery == null) return NotFound();
            
            discovery.DiscoveryStatusId = 4;

            // Ici tu pourrais envoyer un email avec rejectionDto.Reason

            await _discoveryRepository.UpdateAsync(discovery, discovery);

            return NoContent();
        }
    }
}