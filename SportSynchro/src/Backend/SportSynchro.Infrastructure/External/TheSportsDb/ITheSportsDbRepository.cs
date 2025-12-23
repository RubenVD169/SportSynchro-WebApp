using SportSynchro.Infrastructure.External.TheSportsDb.Models.Leagues;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Match;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Sports;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Teams;

namespace SportSynchro.Infrastructure.External.TheSportsDb;

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
