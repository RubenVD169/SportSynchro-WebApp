using SportSynchro.LiveScore.Api.Infrastructure;

namespace SportSynchro.LiveScore.Api.Application;

public sealed class LiveScorePollerService : BackgroundService
{
    private readonly TheSportsDbLiveScoreClient _sportsDb;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LiveScorePollerService> _logger;

    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(120);

    public LiveScorePollerService(
        TheSportsDbLiveScoreClient sportsDb,
        IServiceScopeFactory scopeFactory,
        ILogger<LiveScorePollerService> logger)
    {
        _sportsDb = sportsDb;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LiveScore poller started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                IReadOnlyList<SportsDbLiveScoreInput> liveMatches =
                    await _sportsDb.GetAllLiveScoresAsync(stoppingToken);

                using IServiceScope scope =
                    _scopeFactory.CreateScope();

                LiveScoreIngestService ingest =
                    scope.ServiceProvider.GetRequiredService<LiveScoreIngestService>();

                foreach (SportsDbLiveScoreInput match in liveMatches)
                {
                    await ingest.UpsertFromSportsDbAsync(match, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // normal shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LiveScore poller failed.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
