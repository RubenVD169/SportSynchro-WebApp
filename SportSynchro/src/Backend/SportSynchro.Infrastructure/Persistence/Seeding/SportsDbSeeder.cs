using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.External;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;
using SportSynchro.External.TheSportsDb.Contracts.Models.Leagues;
using SportSynchro.External.TheSportsDb.Contracts.Models.Sports;

namespace SportSynchro.Infrastructure.Persistence.Seeding;

public sealed class SportsDbSeeder : ISportsDbSeeder
{
    private readonly ITheSportsDbRepository _sportsDb;
    private readonly SportSynchroDbContext _db;

    public SportsDbSeeder(
        ITheSportsDbRepository sportsDb,
        SportSynchroDbContext db)
    {
        _sportsDb = sportsDb;
        _db = db;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TheSportsDbSportDto> sports =
            await _sportsDb.GetAllSportsAsync(cancellationToken);

        IReadOnlyList<TheSportsDbLeagueDto> leagues =
            await _sportsDb.GetAllLeaguesAsync(cancellationToken);

        foreach (TheSportsDbSportDto sportDto in sports)
        {
            if (!int.TryParse(sportDto.IdSport, out int sportExternalId))
                continue;

            if (string.IsNullOrWhiteSpace(sportDto.StrSport))
                continue;

            Sport? sport =
                await _db.Sports
                    .FirstOrDefaultAsync(
                        s => s.ExternalId == sportExternalId,
                        cancellationToken);

            if (sport is null)
            {
                sport = new Sport(
                    sportExternalId,
                    SportName.Create(sportDto.StrSport),
                    isVisible: false);

                _db.Sports.Add(sport);
                await _db.SaveChangesAsync(cancellationToken);
            }

            await SeedLeaguesAsync(
                sport,
                leagues,
                cancellationToken);
        }
    }

    private async Task SeedLeaguesAsync(
    Sport sport,
    IReadOnlyList<TheSportsDbLeagueDto> allLeagues,
    CancellationToken ct)
    {
        Dictionary<int, League> existingLeagues =
            await _db.Leagues
                .Where(l => l.SportId == sport.Id)
                .ToDictionaryAsync(l => l.ExternalId, ct);

        List<League> newLeagues = [];

        foreach (TheSportsDbLeagueDto leagueDto in allLeagues)
        {
            if (!int.TryParse(leagueDto.IdLeague, out int leagueExternalId))
                continue;

            if (string.IsNullOrWhiteSpace(leagueDto.StrLeague))
                continue;

            if (!string.Equals(
                    leagueDto.StrSport,
                    sport.Name.Value,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (existingLeagues.ContainsKey(leagueExternalId))
                continue;

            League league = new(
                leagueExternalId,
                LeagueName.Create(leagueDto.StrLeague),
                sport.Id,
                isVisible: false);

            newLeagues.Add(league);
            existingLeagues.Add(leagueExternalId, league);
        }

        if (newLeagues.Count > 0)
        {
            _db.Leagues.AddRange(newLeagues);
            await _db.SaveChangesAsync(ct);
        }
    }

}