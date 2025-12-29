using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Services;

public interface ITeamImportService
{
    Task ImportTeamsForSeasonAsync(
        Season season,
        int leagueExternalId,
        CancellationToken cancellationToken = default);
}
