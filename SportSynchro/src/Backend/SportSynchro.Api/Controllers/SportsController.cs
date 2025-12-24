using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Api.Contracts.Sports.Responses;
using SportSynchro.Api.Mapping.Sports;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Sports;

namespace SportSynchro.Api.Controllers;

[ApiController]
[Route("api/sports")]
[Authorize(Policy = "AdminRead")]
public sealed class SportsController : ControllerBase
{
    private readonly ISportService _sportService;

    public SportsController(ISportService sportService)
    {
        _sportService = sportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        IReadOnlyList<SportAdminModel> models = await _sportService
            .GetAllForAdminAsync(cancellationToken);

        IReadOnlyList<SportAdminResponse> response = models.ToAdminResponses();

        return Ok(response);
    }
}
