using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository.Specific;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DisplayName("NotificationType")]
    public class NotificationTypesController : ReadableController<NotificationType, NotificationTypeDto, NotificationTypeDto, int>
    {
        public NotificationTypesController(INotificationTypeRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}