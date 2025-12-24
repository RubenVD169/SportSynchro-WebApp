namespace SportSynchro.Application.Interfaces.Services;

public interface ILeagueActivationService
{
    Task<bool> SetLeagueVisibilityAsync(
    int leagueId,
    bool isVisible,
    CancellationToken cancellationToken = default);
}
