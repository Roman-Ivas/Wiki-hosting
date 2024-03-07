using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Models.Dto;
using viki_01.Services;

namespace viki_01.Controllers;

[ApiController]
[Route("/[controller]")]
public class ReportController(IReportRepository reportRepository) : ControllerBase
{
    [HttpGet("/search/{userId:int?}")]
    [Authorize("ModerationOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetReports([FromRoute] int? userId = null, [FromQuery] string? contentUrl = null)
    {
        var reports = await reportRepository.GetAllAsync(userId, contentUrl);
        return Ok(reports);
    }
    
    [HttpGet("{id:int}")]
    [Authorize("ModerationOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetReport([FromRoute] int id)
    {
        var report = await reportRepository.GetAsync(id);
        return Ok(report);
    }
    
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReport([FromBody] ReportUpsertDto reportUpsertDto, [FromServices] IMapper<Report, ReportUpsertDto> mapper)
    {
        var report = mapper.Map(reportUpsertDto);
        report.PostedAt = DateTime.Now;
        report.UserId = HttpContext.User.GetId();
        
        await reportRepository.CreateAsync(report);
        return Ok();
    }
}