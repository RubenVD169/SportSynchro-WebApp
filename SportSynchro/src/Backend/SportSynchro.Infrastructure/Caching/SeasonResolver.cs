using Microsoft.Extensions.Caching.Memory;
using SportSynchro.Application.Interfaces.Lookups;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Infrastructure.Caching;

public sealed class SeasonResolver : ISeasonResolver
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly IMemoryCache _cache;

    private static string CacheKey(int leagueId)
        => $"current-season:{leagueId}";

    // Long cache duration as seasons change infrequently and invalidation happens on season changes
    private static readonly MemoryCacheEntryOptions CacheOptions =
        new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        };

    public SeasonResolver(
        ISeasonRepository seasonRepository,
        IMemoryCache cache)
    {
        _seasonRepository = seasonRepository;
        _cache = cache;
    }

    public async Task<int> GetCurrentSeasonIdAsync(
        int leagueId,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey(leagueId), out int seasonId))
            return seasonId;

        Season season =
            await _seasonRepository.GetCurrentForLeagueAsync(
                leagueId,
                cancellationToken) ?? throw new SeasonException(
                $"No current season found for league {leagueId}.");
        _cache.Set(CacheKey(leagueId), season.Id, CacheOptions);

        return season.Id;
    }

    public void Invalidate(int leagueId)
        => _cache.Remove(CacheKey(leagueId));
}
