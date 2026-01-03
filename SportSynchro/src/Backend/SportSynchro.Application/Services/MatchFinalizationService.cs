using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Matches;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Services;

public sealed class MatchFinalizationService : IMatchFinalizationService
{
    private readonly IMatchRepository _matches;

    public MatchFinalizationService(IMatchRepository matches)
    {
        _matches = matches;
    }

    public async Task HandleFinishedAsync(
        MatchFinishedModel model,
        CancellationToken ct)
    {
        int externalId = int.Parse(model.EventId);

        Match? match = await _matches.GetByExternalIdAsync(externalId, ct);

        if (match is null)
        {
            return;
        }
        match.Finish(model.HomeScore, model.AwayScore);

        await _matches.SaveChangesAsync(ct);
    }

    public async Task HandleFinishedBatchAsync(
    IReadOnlyList<MatchFinishedModel> batch,
    CancellationToken ct)
    {
        if (batch.Count == 0)
            return;

        int[] externalIds = [.. batch.Select(m => int.Parse(m.EventId))];

        List<Match> matches =
            await _matches.GetByExternalIdsAsync(externalIds, ct);

        Dictionary<int, Match> lookup =
            matches.ToDictionary(m => m.ExternalId);

        foreach (MatchFinishedModel item in batch)
        {
            int externalId = int.Parse(item.EventId);

            if (!lookup.TryGetValue(externalId, out Match? match))
                continue;

            if (match.Status.Value == "Finished")
                continue;

            match.Finish(item.HomeScore, item.AwayScore);
        }

        await _matches.SaveChangesAsync(ct);
    }

}