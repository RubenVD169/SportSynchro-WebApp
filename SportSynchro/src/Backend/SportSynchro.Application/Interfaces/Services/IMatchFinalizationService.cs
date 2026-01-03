using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Application.Interfaces.Services;

public interface IMatchFinalizationService
{
    Task HandleFinishedAsync(MatchFinishedModel model, CancellationToken ct);
    Task HandleFinishedBatchAsync(IReadOnlyList<MatchFinishedModel> models, CancellationToken ct);
}