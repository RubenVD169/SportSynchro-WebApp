using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Application.Interfaces.Services;

public interface IMatchService
{
    Task <IReadOnlyList<MatchModel>> GetRecentMatchesByLeagueIdAsync(int leagueId, CancellationToken cancellationToken);
}