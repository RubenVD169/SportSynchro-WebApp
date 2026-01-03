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

        if (_cache.TryGetValue(cacheKey, out int externalId))
        {
            return externalId.ToString();
        }

        int dbExternalId = await _db.Leagues
            .AsNoTracking()                   
            .Where(l => l.Id == internalLeagueId)
            .Select(l => l.ExternalId)
            .SingleOrDefaultAsync(ct);
        
        string dbExternalIdStr = dbExternalId.ToString() ?? throw new InvalidOperationException(
                $"League '{internalLeagueId}' has no ExternalId.");
        
        _cache.Set(cacheKey, dbExternalIdStr, TimeSpan.FromHours(1));
        return dbExternalIdStr;
    }
}