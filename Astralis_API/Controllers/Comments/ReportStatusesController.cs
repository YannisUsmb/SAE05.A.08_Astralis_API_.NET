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
    [DisplayName("Report Status")]
    public class ReportStatusesController : ReadableController<ReportStatus, ReportStatusDto, ReportStatusDto, int>
    {
        public ReportStatusesController(IReportStatusRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}