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
        bool isFinished = input.StrStatus == "FT";
        string id = $"event-{input.IdEvent}";
        string partitionKey = input.IdLeague;
        
        // Match is finished → check existing and notify if needed
        if (isFinished)
        {
            LiveMatchDocument? existing =
                await _repository.TryGetAsync(id, partitionKey, ct);

            LiveMatchDocument document =
                existing is null
                    ? LiveMatchDocument.CreateFromSportsDb(input)
                    : existing.WithUpdatedSnapshot(
                        input,
                        DetermineTtl(input.StrStatus));

            if (!document.FinishedNotified)
            {
                await _sportSynchroApi.NotifyMatchFinishedAsync(
                    new MatchFinishedRequest
                    {
                        EventId = input.IdEvent,
                        HomeScore = int.TryParse(input.IntHomeScore, out int hs) ? hs : null,
                        AwayScore = int.TryParse(input.IntAwayScore, out int aw) ? aw : null,
                    },
                    ct);

                document.FinishedNotified = true;
            }

            document.TimeToLiveSeconds = DetermineTtl(input.StrStatus);
            await _repository.UpsertAsync(document, ct);
            return;
        }

        // Not FT → blind upsert
        LiveMatchDocument liveDocument =
            LiveMatchDocument.CreateFromSportsDb(input);

        await _repository.UpsertAsync(liveDocument, ct);
    }

    private static int? DetermineTtl(string? status)
        => status == "FT" ? 172_800 : null;
}
