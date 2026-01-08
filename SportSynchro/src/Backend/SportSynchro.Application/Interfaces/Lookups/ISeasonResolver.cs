namespace SportSynchro.Application.Interfaces.Lookups;

public interface ISeasonResolver
{
    Task<int> GetCurrentSeasonIdAsync(
        int leagueId,
        CancellationToken cancellationToken = default);

    void Invalidate(int leagueId);
}
