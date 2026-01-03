using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Application.Services;

public sealed class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    public MatchService(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }
    public async Task<IReadOnlyList<MatchModel>> GetRecentMatchesByLeagueIdAsync(
        int leagueId, CancellationToken cancellationToken)
    {
        IReadOnlyList<MatchModel> matches = await _matchRepository.GetRecentFinishedMatchesByLeagueIdAsync(leagueId, cancellationToken);
        return matches;
    }

    public async Task<IReadOnlyList<MatchModel>> GetScheduledMatchesByLeagueIdAsync(
        int leagueId, CancellationToken cancellationToken)
    {
        IReadOnlyList<MatchModel> matches = await _matchRepository.GetScheduledMatchesByLeagueIdAsync(leagueId, cancellationToken);
        return matches;
    }
}