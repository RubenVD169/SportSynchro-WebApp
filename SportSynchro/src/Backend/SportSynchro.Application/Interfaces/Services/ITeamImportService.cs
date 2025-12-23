using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Services;

public interface ITeamImportService
{
    Task ImportTeamsForLeagueAsync(
        League league,
        CancellationToken cancellationToken = default);
}
