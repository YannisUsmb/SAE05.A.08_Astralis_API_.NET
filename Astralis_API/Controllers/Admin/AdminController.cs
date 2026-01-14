using Astralis_API.Models.EntityFramework;
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
        private readonly AstralisDbContext _context;
        private readonly IMapper _mapper;

        public AdminController(IDiscoveryRepository discoveryRepository, IMapper mapper, AstralisDbContext context)
        {
            _discoveryRepository = discoveryRepository;
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Admin/Discoveries/Pending
        [HttpGet("Discoveries/Pending")]
        public async Task<ActionResult<IEnumerable<DiscoveryDto>>> GetPendingDiscoveries()
        {
            var pendingDiscoveries = await _discoveryRepository.SearchAsync(discoveryStatusId: 2); 
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
            
            var notification = new Notification
            {
                NotificationTypeId = 6, 
                Label = "Découverte Validée", 
                Description = $"Votre découverte '{discovery.Title}' a été validée et est maintenant publique."
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            
            var userNotif = new UserNotification
            {
                UserId = discovery.UserId,
                NotificationId = notification.Id,
                IsRead = false,
                ReceivedAt = DateTime.UtcNow
            };

            _context.UserNotifications.Add(userNotif);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Admin/Discoveries/{id}/Reject
        [HttpPost("Discoveries/{id}/Reject")]
        public async Task<IActionResult> RejectDiscovery(int id, [FromBody] DiscoveryRejectionDto rejectionDto)
        {
            var discovery = await _discoveryRepository.GetByIdAsync(id);
            if (discovery == null) return NotFound();
            
            discovery.DiscoveryStatusId = 4;
            await _discoveryRepository.UpdateAsync(discovery, discovery);


            var notification = new Notification
            {
                NotificationTypeId = 6, 
                Label = "Découverte Refusée",
                Description = $"Votre découverte '{discovery.Title}' a été refusée. Motif : {rejectionDto.Reason}"
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            
            var userNotif = new UserNotification
            {
                UserId = discovery.UserId,
                NotificationId = notification.Id,
                IsRead = false,
                ReceivedAt = DateTime.UtcNow
            };

            _context.UserNotifications.Add(userNotif);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}