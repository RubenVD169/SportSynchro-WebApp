using Microsoft.Extensions.Caching.Memory;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Application.Services;

public sealed class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMemoryCache _cache;

    public MatchService(IMatchRepository matchRepository, IMemoryCache cache)
    {
        _matchRepository = matchRepository;
        _cache = cache;
    }
    
    public async Task<IReadOnlyList<MatchModel>> GetScheduledMatchesByLeagueIdAsync(
        int leagueId, CancellationToken cancellationToken)
    {
        IReadOnlyList<MatchModel> matches = await _matchRepository.GetScheduledMatchesByLeagueIdAsync(leagueId, cancellationToken);
        return matches;
    }

    public async Task<IReadOnlyList<MatchModel>> GetRecentFinishedMatchesForVisibleLeaguesBySportIdAsync(
            int sportId,
            CancellationToken cancellationToken)
    {
        string cacheKey = $"recent-finished-matches:sport:{sportId}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<MatchModel>? cached) && cached is not null)
        {
            return cached;
        }


        IReadOnlyList<MatchModel> matches =
            await _matchRepository
                .GetRecentFinishedMatchesForVisibleLeaguesBySportIdAsync(
                    sportId,
                    cancellationToken);

        _cache.Set(
            cacheKey,
            matches,
            TimeSpan.FromMinutes(10));

        return matches;
    }
}