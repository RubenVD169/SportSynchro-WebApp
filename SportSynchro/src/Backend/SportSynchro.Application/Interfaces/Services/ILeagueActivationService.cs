using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Services;

public interface ILeagueActivationService
{
    Task SetLeagueVisibilityAsync(
        League league,
        bool isVisible,
        CancellationToken cancellationToken = default);
}
