using SportSynchro.LiveScore.Api.Infrastructure;
using SportSynchro.LiveScore.Api.Models;

namespace SportSynchro.LiveScore.Api.Application;

public sealed class LiveScorePollerService : BackgroundService
{
    private readonly ILogger<LiveScorePollerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TheSportsDbLiveScoreClient _sportsDb;
    private readonly SportSynchroApiClient _sportSynchroApi;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(120);

    public LiveScorePollerService(
        ILogger<LiveScorePollerService> logger,
        IServiceScopeFactory scopeFactory,
        TheSportsDbLiveScoreClient sportsDb,
        SportSynchroApiClient sportSynchroApi)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _sportsDb = sportsDb;
        _sportSynchroApi = sportSynchroApi;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                IReadOnlyList<SportsDbLiveScoreInput> liveMatches =
                    await _sportsDb.GetAllLiveScoresAsync(stoppingToken);

                using IServiceScope scope = _scopeFactory.CreateScope();

                LiveScoreIngestService ingestService =
                    scope.ServiceProvider.GetRequiredService<LiveScoreIngestService>();

                IReadOnlyList<MatchFinishedRequest> finishedBatch =
                    await ingestService.UpsertFromSportsDbAsync(
                        liveMatches,
                        stoppingToken);

                if (finishedBatch.Count > 0)
                {
                    await _sportSynchroApi.NotifyMatchesFinishedBatchAsync(
                        finishedBatch,
                        stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LiveScore poller failed");
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }
    }
}
