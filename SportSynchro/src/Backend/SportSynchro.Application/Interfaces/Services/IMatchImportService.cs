using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Services;

public interface IMatchImportService
{
  // Imports matches for a given league and returns the (max) StartTimeUtc that was imported (latest match)
  Task<DateTime?> ImportMatchesForLeagueAsync(
       League league,
       CancellationToken cancellationToken = default);
}