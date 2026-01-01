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
}