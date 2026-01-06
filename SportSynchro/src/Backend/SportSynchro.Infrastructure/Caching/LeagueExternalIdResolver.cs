using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportSynchro.Application.Interfaces.Lookups;
using SportSynchro.Infrastructure.Persistence;

namespace SportSynchro.Infrastructure.Caching;

public sealed class LeagueExternalIdResolver: ILeagueExternalIdResolver
{
    private readonly SportSynchroDbContext _db;
    private readonly IMemoryCache _cache;

    public LeagueExternalIdResolver(
        SportSynchroDbContext db,
        IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<string> GetExternalLeagueIdAsync(
    int internalLeagueId,
    CancellationToken ct)
    {
        string cacheKey = $"league:external:{internalLeagueId}";

        if (_cache.TryGetValue(cacheKey, out string? cached)
            && cached is not null)
        {
            return cached;
        }

        int dbExternalId = await _db.Leagues
            .AsNoTracking()
            .Where(l => l.Id == internalLeagueId)
            .Select(l => l.ExternalId)
            .SingleOrDefaultAsync(ct);

        if (dbExternalId == 0)
            throw new InvalidOperationException(
                $"League '{internalLeagueId}' has no ExternalId.");

        string dbExternalIdStr = dbExternalId.ToString();

        _cache.Set(
            cacheKey,
            dbExternalIdStr);

        return dbExternalIdStr;
    }
}