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
    private readonly ISeasonTeamRepository _seasonTeamRepository;

    public TeamImportService(
        ITheSportsDbRepository sportsDb,
        ITeamRepository teamRepository,
        ISeasonTeamRepository seasonTeamRepository)
    {
        _sportsDb = sportsDb;
        _teamRepository = teamRepository;
        _seasonTeamRepository = seasonTeamRepository;
    }

    public async Task ImportTeamsForSeasonAsync(
        Season season,
        int leagueExternalId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TheSportsDbTeamDto> apiTeams =
            await _sportsDb.GetTeamsByLeagueAsync(
                leagueExternalId,
                cancellationToken);

        if (apiTeams.Count == 0)
            return;

        // ExternalId -> Team (existing teams in DB)
        Dictionary<int, Team> existingTeams =
            await _teamRepository.GetByExternalIdsAsync(
                apiTeams
                    .Select(t => int.TryParse(t.IdTeam, out int id) ? id : -1)
                    .Where(id => id > 0)
                    .ToHashSet(),
                cancellationToken);

        // TeamIds already linked to this season
        HashSet<int> existingTeamIdsForSeason =
            await _seasonTeamRepository.GetTeamIdsForSeasonAsync(
                season.Id,
                cancellationToken);

        List<Team> newTeams = [];
        List<SeasonTeam> newSeasonTeams = [];

        // Detect new teams
        foreach (TheSportsDbTeamDto apiTeam in apiTeams)
        {
            if (!int.TryParse(apiTeam.IdTeam, out int externalId))
                continue;

            if (string.IsNullOrWhiteSpace(apiTeam.StrTeam))
                continue;

            if (existingTeams.ContainsKey(externalId))
                continue;

            Team team = new(
                externalId,
                TeamName.Create(apiTeam.StrTeam),
                apiTeam.StrCountry ?? "Unknown");

            newTeams.Add(team);
            existingTeams[externalId] = team;
        }

        // Save new teams in bulk
        if (newTeams.Count > 0)
        {
            await _teamRepository.AddRangeAsync(
                newTeams,
                cancellationToken);

            await _teamRepository.SaveChangesAsync(cancellationToken);
        }

        // Create SeasonTeam records 
        newSeasonTeams.AddRange(from team in existingTeams.Values 
            where !existingTeamIdsForSeason.Contains(team.Id) select new SeasonTeam(season.Id, team.Id));

        foreach (SeasonTeam seasonTeam in newSeasonTeams)
        {
            await _seasonTeamRepository.AddAsync(
                seasonTeam,
                cancellationToken);
        }

        if (newSeasonTeams.Count > 0)
        {
            await _seasonTeamRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
