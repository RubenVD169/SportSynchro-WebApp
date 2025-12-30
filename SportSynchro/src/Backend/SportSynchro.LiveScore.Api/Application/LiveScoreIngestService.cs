using SportSynchro.LiveScore.Api.Infrastructure;
using SportSynchro.LiveScore.Api.Models;

namespace SportSynchro.LiveScore.Api.Application;

public sealed class LiveScoreIngestService
{
    private readonly ILiveMatchRepository _repository;
    private readonly SportSynchroApiClient _sportSynchroApi;

    public LiveScoreIngestService(
        ILiveMatchRepository repository,
        SportSynchroApiClient sportSynchroApi)
    {
        _repository = repository;
        _sportSynchroApi = sportSynchroApi;
    }

    public async Task UpsertFromSportsDbAsync(
        SportsDbLiveScoreInput input,
        CancellationToken ct = default)
    {
        LiveMatchDocument document = new()
        {
            Id = $"event-{input.IdEvent}",
            IdEvent = input.IdEvent,
            IdLiveScore = input.IdLiveScore,
            IdLeague = input.IdLeague,

            HomeTeamId = input.IdHomeTeam,
            HomeTeamName = input.StrHomeTeam,
            AwayTeamId = input.IdAwayTeam,
            AwayTeamName = input.StrAwayTeam,

            HomeScore = input.IntHomeScore,
            AwayScore = input.IntAwayScore,

            Status = input.StrStatus,
            Progress = input.StrProgress,
            UpdatedRaw = input.Updated,

            TimeToLiveSeconds = DetermineTtl(input.StrStatus),
        };

        if (input.StrStatus == "FT" && !document.FinishedNotified)
        {
            await _repository.UpsertAsync(document, ct); await _sportSynchroApi.NotifyMatchFinishedAsync(
                 new MatchFinishedRequest
                 {
                     EventId = input.IdEvent,
                     LeagueId = input.IdLeague,
                     HomeTeamId = input.IdHomeTeam,
                     AwayTeamId = input.IdAwayTeam,
                     HomeScore = int.TryParse(input.IntHomeScore, out int homeScore) ? homeScore : null,
                     AwayScore = int.TryParse(input.IntAwayScore, out int awayScore) ? awayScore : null,
                     FinishedAtUtc = DateTime.UtcNow
                 },
                 ct);
            document.FinishedNotified = true;

            await _repository.UpsertAsync(document, ct); 
        }
    }

    private static int? DetermineTtl(string? status)
        => status == "FT" ? 300 : null;
}