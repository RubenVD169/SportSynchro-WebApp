using Microsoft.AspNetCore.Mvc;
using SportSynchro.Application.SportsSeeding.Abstractions;

namespace SportSynchro.Api.Controllers.Dev;

[ApiController]
[Route("api/dev/seed")]
public sealed class SeedController : ControllerBase
{
    private readonly ISportsDbSeeder _seeder;
    private readonly ILogger<SeedController> _logger;

    public SeedController(
        ISportsDbSeeder seeder,
        ILogger<SeedController> logger)
    {
        _seeder = seeder;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Seed(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Manual sports seeding started.");

        await _seeder.SeedAsync(cancellationToken);

        _logger.LogInformation("Manual sports seeding finished.");

        return Ok(new
        {
            status = "ok",
            message = "Sports database seeding completed."
        });
    }
}
