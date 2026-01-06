using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Application.Interfaces.Services;

public interface IMatchService
{
    Task<IReadOnlyList<MatchModel>> GetScheduledMatchesByLeagueIdAsync(
        int leagueId, CancellationToken cancellationToken);
    Task<IReadOnlyList<MatchModel>> GetRecentFinishedMatchesForVisibleLeaguesBySportIdAsync(
        int sportId, CancellationToken cancellationToken);

}