using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DisplayName("Report Motive")]
    public class ReportMotivesController : ReadableController<ReportMotive, ReportMotiveDto, ReportMotiveDto, int>
    {
        public ReportMotivesController(IReportMotiveRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}