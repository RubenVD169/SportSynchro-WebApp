using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Sports;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.Exceptions;

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

    public async Task SetVisibilityAsync(
        int sportId,
        bool isVisible,
        CancellationToken cancellationToken = default)
    {
        Sport sport = await _sportRepository
            .GetByIdAsync(sportId, cancellationToken) 
                       ?? throw new SportException($"Sport with id {sportId} was not found.");
        sport.SetVisibility(isVisible);

        await _sportRepository
            .SaveChangesAsync(cancellationToken);
    }
}
