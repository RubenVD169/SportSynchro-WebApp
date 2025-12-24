using Microsoft.Extensions.Options;
using SportSynchro.Api.Options;
using SportSynchro.Infrastructure.Persistence.Seeding;

namespace SportSynchro.Api.Workers;

public sealed class SportsSeedingWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SportsSeedingWorker> _logger;
    private readonly SportsSeedingOptions _options;

    public SportsSeedingWorker(
        IServiceProvider serviceProvider,
        ILogger<SportsSeedingWorker> logger,
        IOptions<SportsSeedingOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation(
                "SportsSeedingWorker disabled via configuration.");

            return; //When disabled, exit immediately => RIP AZURE LIMITED FREE PLAN :(
        }

        using IServiceScope scope = _serviceProvider.CreateScope();
        ISportsDbSeeder seeder = scope.ServiceProvider.GetRequiredService<ISportsDbSeeder>();

        _logger.LogInformation("Sports seeding started");

        await seeder.SeedAsync(stoppingToken);

        _logger.LogInformation("Sports seeding finished");
    }
}
