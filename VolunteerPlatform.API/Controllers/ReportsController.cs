using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.DTOs.Request;
using VolunteerPlatform.Service.DTOs.Response;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<ActionResult<ReportDto>> CreateReport([FromBody] CreateReportRequestDto request, [FromHeader(Name = "X-User-Id")] Guid reporterId)
    {
        if (reporterId == Guid.Empty)
        {
            return BadRequest("X-User-Id header is required.");
        }

        var result = await _reportService.CreateReportAsync(reporterId, request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportDto>>> GetAllReports()
    {
        var reports = await _reportService.GetAllReportsAsync();
        return Ok(reports);
    }

    [HttpPut("{id}/resolve")]
    public async Task<IActionResult> ResolveReport(Guid id)
    {
        await _reportService.ResolveReportAsync(id);
        return NoContent();
    }
}
