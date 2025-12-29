using SportSynchro.Application.Interfaces.External;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;
using SportSynchro.External.TheSportsDb.Contracts.Models.Teams;

namespace SportSynchro.Application.Services;

public sealed class TeamImportService : ITeamImportService
{
    private readonly ITheSportsDbRepository _sportsDb;
    private readonly ITeamRepository _teamRepository;

    public TeamImportService(
        ITheSportsDbRepository sportsDb,
        ITeamRepository teamRepository)
    {
        _sportsDb = sportsDb;
        _teamRepository = teamRepository;
    }

    public async Task ImportTeamsForLeagueAsync(
        League league,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TheSportsDbTeamDto> apiTeams =
            await _sportsDb.GetTeamsByLeagueAsync(
                league.ExternalId,
                cancellationToken);

        if (apiTeams.Count == 0)
            return;

        HashSet<int> existingExternalIds =
            await _teamRepository
                .GetExistingExternalIdsForLeagueAsync(
                    league.Id,
                    cancellationToken);

        List<Team> newTeams = [];

        foreach (TheSportsDbTeamDto apiTeam in apiTeams)
        {
            if (!int.TryParse(apiTeam.IdTeam, out int externalId))
                continue;

            if (string.IsNullOrWhiteSpace(apiTeam.StrTeam))
                continue;

            if (existingExternalIds.Contains(externalId))
                continue;

            Team team = new(
                externalId,
                TeamName.Create(apiTeam.StrTeam),
                apiTeam.StrCountry ?? "Unknown");

            newTeams.Add(team);
        }

        if (newTeams.Count > 0)
        {
            await _teamRepository
                .AddRangeAsync(newTeams, cancellationToken);

            await _teamRepository
                .SaveChangesAsync(cancellationToken);
        }
    }
}