using Microsoft.Extensions.Caching.Memory;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Sports;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Services;

public sealed class SportService : ISportService
{
    private readonly ISportRepository _sportRepository;
    private readonly IMemoryCache _cache;
    private const string AdminSportsCacheKey = "admin-sports-all";
    private const string UserSportsCacheKey = "visible-sports";


    public SportService(ISportRepository sportRepository, IMemoryCache memoryCache)
    {
        _sportRepository = sportRepository;
        _cache = memoryCache;
    }

    public async Task<IReadOnlyList<SportAdminModel>> GetAllForAdminAsync(
    CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(
                AdminSportsCacheKey,
                out IReadOnlyList<SportAdminModel>? cached)
            && cached is not null)
        {
            return cached;
        }

        IReadOnlyList<Sport> sports =
            await _sportRepository.GetAllAsync(cancellationToken);

        IReadOnlyList<SportAdminModel> result =
        [
            .. sports.Select(s => new SportAdminModel(
            s.Id,
            s.Name.Value,
            s.IsVisible))
        ];

        // No absolute expiration → remains until explicit invalidation
        _cache.Set(AdminSportsCacheKey, result);

        return result;
    }


    public async Task<IReadOnlyList<SportUserModel>> GetAllForUserAsync(
         CancellationToken cancellationToken)
    {
        const string cacheKey = "visible-sports";

        if (_cache.TryGetValue(
                cacheKey,
                out IReadOnlyList<SportUserModel>? cached)
            && cached is not null)
        {
            return cached;
        }

        IReadOnlyList<Sport> sports =
            await _sportRepository.GetAllVisibleAsync(cancellationToken);

        IReadOnlyList<SportUserModel> result =
        [
            .. sports.Select(s => new SportUserModel(
                s.Id,
                s.Name.Value))
        ];

        _cache.Set(cacheKey, result);

        return result;
    }

    public async Task<bool> SetSportVisibilityAsync(
    int sportId,
    bool isVisible,
    CancellationToken cancellationToken = default)
    {
        Sport? sport = await _sportRepository
            .GetByIdAsync(sportId, cancellationToken);

        if (sport is null)
            return false;

        sport.SetVisibility(isVisible);

        int result = await _sportRepository
            .SaveChangesAsync(cancellationToken);

        if (result == 0) return false;
        // Invalidate caches
        _cache.Remove(AdminSportsCacheKey);
        _cache.Remove(UserSportsCacheKey);

        return true;
    }
}
