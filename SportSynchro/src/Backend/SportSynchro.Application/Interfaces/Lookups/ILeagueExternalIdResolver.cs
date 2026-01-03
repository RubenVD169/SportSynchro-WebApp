namespace SportSynchro.Application.Interfaces.Lookups;

public interface ILeagueExternalIdResolver
{
  Task<string> GetExternalLeagueIdAsync(
    int internalLeagueId,
    CancellationToken ct);
}