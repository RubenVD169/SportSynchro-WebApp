using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;
using SportSynchro.Infrastructure.External.TheSportsDb;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Teams;
using SportSynchro.Infrastructure.Persistence;

namespace SportSynchro.Application.Services;

public sealed class TeamImportService : ITeamImportService
{
    private readonly ITheSportsDbRepository _sportsDb;
    private readonly SportSynchroDbContext _db;

    public TeamImportService(
        ITheSportsDbRepository sportsDb,
        SportSynchroDbContext db)
    {
        _sportsDb = sportsDb;
        _db = db;
    }

    public async Task ImportTeamsForLeagueAsync(
        League league,
        CancellationToken cancellationToken = default)
    {
        // Get teams from external TheSportsDb API
        IReadOnlyList<TheSportsDbTeamDto> apiTeams =
            await _sportsDb.GetTeamsByLeagueAsync(
                league.ExternalId,
                cancellationToken);

        if (apiTeams.Count == 0)
            return;

        // get existing teams for this league only once
        HashSet<int> existingExternalIds =
            await _db.Teams
                .Where(t => t.LeagueId == league.Id)
                .Select(t => t.ExternalId)
                .ToHashSetAsync(cancellationToken);

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
                apiTeam.StrCountry ?? "Unknown",
                league.Id);

            newTeams.Add(team);
        }

        // Batch insert
        if (newTeams.Count > 0)
        {
            _db.Teams.AddRange(newTeams);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
