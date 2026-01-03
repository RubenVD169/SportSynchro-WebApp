using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Services;

public interface IMatchImportService
{
    // Imports matches for a given season and returns the latest StartTimeUtc imported
    Task<DateTime?> ImportMatchesForSeasonAsync(
        Season season,
        int leagueExternalId,
        CancellationToken cancellationToken = default);
}
