using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Api.Contracts.Sports.Responses;
using SportSynchro.Api.Mapping.Sports;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Sports;

namespace SportSynchro.Api.Controllers.User;

[ApiController]
[Route("api/user/sports")]
public sealed class UserSportsController : ControllerBase
{
    private readonly ISportService _sportService;

    public UserSportsController(ISportService sportService)
    {
        _sportService = sportService;
    }

    [Authorize(policy: "UserRead")]
    [HttpGet]
    public async Task<IActionResult> GetAllForUser(
        CancellationToken cancellationToken)
    {
        IReadOnlyList<SportUserModel> models = await _sportService
            .GetAllForUserAsync(cancellationToken);

        IReadOnlyList<SportUserResponse> response = models.ToUserResponses();

        return Ok(response);
    }
}