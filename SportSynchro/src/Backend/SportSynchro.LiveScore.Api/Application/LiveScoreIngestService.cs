using SportSynchro.LiveScore.Api.Infrastructure;
using SportSynchro.LiveScore.Api.Models;

namespace SportSynchro.LiveScore.Api.Application;

public sealed class LiveScoreIngestService
{
    private readonly ILiveMatchRepository _repository;

    public LiveScoreIngestService(ILiveMatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MatchFinishedRequest>> UpsertFromSportsDbAsync(
        IReadOnlyList<SportsDbLiveScoreInput> inputs,
        CancellationToken ct = default)
    {
        List<MatchFinishedRequest> finishedBatch = [];

        foreach (SportsDbLiveScoreInput input in inputs)
        {
            bool isFinished = input.StrStatus == "FT";
            string id = $"event-{input.IdEvent}";
            string partitionKey = input.IdLeague;

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
                    finishedBatch.Add(new MatchFinishedRequest
                    {
                        EventId = input.IdEvent,
                        HomeScore = int.TryParse(input.IntHomeScore, out int hs) ? hs : null,
                        AwayScore = int.TryParse(input.IntAwayScore, out int aw) ? aw : null,
                    });

                    document.FinishedNotified = true;
                }

                document.TimeToLiveSeconds = DetermineTtl(input.StrStatus);
                await _repository.UpsertAsync(document, ct);
                continue;
            }

            // Not FT → blind upsert
            LiveMatchDocument liveDocument =
                LiveMatchDocument.CreateFromSportsDb(input);

            await _repository.UpsertAsync(liveDocument, ct);
        }

        return finishedBatch;
    }

    private static int? DetermineTtl(string? status)
        => status == "FT" ? 172_800 : null;
}
