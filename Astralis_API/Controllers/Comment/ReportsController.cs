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
    [DisplayName("Report")]
    public class ReportsController : CrudController<Report, ReportDto, ReportDto, ReportCreateDto, ReportUpdateDto, int>
    {
        public ReportsController(IReportRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}