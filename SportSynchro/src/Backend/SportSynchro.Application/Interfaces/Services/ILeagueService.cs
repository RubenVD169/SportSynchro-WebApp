using SportSynchro.Application.Models.Leagues;

namespace SportSynchro.Application.Interfaces.Services;

public interface ILeagueService
{
    Task<IReadOnlyList<LeagueAdminModel>> GetLeaguesForAdminBySportIdAsync(int sportId, CancellationToken cancellationToken);
    Task<bool> SetLeagueVisibilityAsync(
    int leagueId,
    bool isVisible,
    CancellationToken cancellationToken = default);
}
