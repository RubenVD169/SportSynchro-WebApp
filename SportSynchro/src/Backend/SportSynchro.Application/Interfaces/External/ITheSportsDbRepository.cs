using SportSynchro.External.TheSportsDb.Contracts.Models.Leagues;
using SportSynchro.External.TheSportsDb.Contracts.Models.Matches;
using SportSynchro.External.TheSportsDb.Contracts.Models.Sports;
using SportSynchro.External.TheSportsDb.Contracts.Models.Teams;

namespace SportSynchro.Application.Interfaces.External;

public interface ITheSportsDbRepository
{
    // Sports
    Task<IReadOnlyList<TheSportsDbSportDto>> GetAllSportsAsync(
        CancellationToken cancellationToken = default);

    // Leagues
    Task<IReadOnlyList<TheSportsDbLeagueDto>> GetAllLeaguesAsync(
        CancellationToken cancellationToken = default);

    // Teams
    Task<IReadOnlyList<TheSportsDbTeamDto>> GetTeamsByLeagueAsync(
        int leagueExternalId,
        CancellationToken cancellationToken = default);

    // Seasons
    Task<IReadOnlyList<string>> GetSeasonsByLeagueAsync(
        int leagueExternalId,
        CancellationToken cancellationToken = default);

    // Matches (league + season)
    Task<IReadOnlyList<TheSportsDbMatchDto>> GetMatchesByLeagueAndSeasonAsync(
        int leagueExternalId,
        string season,
        CancellationToken cancellationToken = default);
}
