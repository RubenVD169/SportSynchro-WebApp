using SportSynchro.LiveScore.Api.Models;

namespace SportSynchro.LiveScore.Api.Infrastructure;

public interface ILiveMatchRepository
{
    Task UpsertAsync(LiveMatchDocument match, CancellationToken ct = default);

    Task<IReadOnlyList<LiveMatchDocument>>
        GetByLeagueAsync(string leagueId, CancellationToken ct = default);

    Task<LiveMatchDocument?>
        GetByEventAsync(string eventId, string leagueId, CancellationToken ct = default);
}
