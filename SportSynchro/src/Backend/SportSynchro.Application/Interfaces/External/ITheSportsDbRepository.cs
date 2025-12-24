using SportSynchro.External.TheSportsDb.Contracts.Models.Leagues;
using SportSynchro.External.TheSportsDb.Contracts.Models.Matches;
using SportSynchro.External.TheSportsDb.Contracts.Models.Sports;
using SportSynchro.External.TheSportsDb.Contracts.Models.Teams;

namespace SportSynchro.Application.Interfaces.External;

public interface ITheSportsDbRepository
{
    Task<IReadOnlyList<TheSportsDbSportDto>> GetAllSportsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TheSportsDbLeagueDto>> GetAllLeaguesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TheSportsDbTeamDto>> GetTeamsByLeagueAsync(
        int leagueExternalId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TheSportsDbMatchDto>> GetMatchesByTeamAsync(
        int teamExternalId,
        CancellationToken cancellationToken = default);
}
