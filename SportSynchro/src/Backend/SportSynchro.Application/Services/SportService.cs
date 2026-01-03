using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Sports;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Services;

public sealed class SportService : ISportService
{
    private readonly ISportRepository _sportRepository;

    public SportService(ISportRepository sportRepository)
    {
        _sportRepository = sportRepository;
    }

    public async Task<IReadOnlyList<SportAdminModel>> GetAllForAdminAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Sport> sports = await _sportRepository
            .GetAllAsync(cancellationToken);

        return [.. sports
            .Select(s => new SportAdminModel(
                s.Id,
                s.Name.Value,
                s.IsVisible
            ))];
    }

    public async Task<IReadOnlyList<SportUserModel>> GetAllForUserAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Sport> sports = await _sportRepository
            .GetAllVisibleAsync(cancellationToken);
        
        return [.. sports
            .Select(s => new SportUserModel(
                s.Id,
                s.Name.Value
            ))];
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

        return result != 0;
    }
}
