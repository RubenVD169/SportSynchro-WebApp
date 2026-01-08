using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Application.Interfaces.Blob;

namespace SportSynchro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
  private readonly ISchedulePdfService _schedulePdfService;
  public ScheduleController(ISchedulePdfService schedulePdfService)
  {
    _schedulePdfService = schedulePdfService;
  }
  
  [Authorize(policy: "UserRead")]
  [HttpGet("{leagueId:int}")]
  public async Task<IActionResult> GetSchedule([FromRoute] int leagueId, CancellationToken cancellationToken)
  {
    byte[] pdf = await _schedulePdfService.GetSchedulePdfAsync(leagueId, cancellationToken);
    if (pdf.Length == 0)
    {
      return NotFound();
    }
    return File(pdf, "application/pdf", $"schedule_{leagueId}.pdf");
  }
}