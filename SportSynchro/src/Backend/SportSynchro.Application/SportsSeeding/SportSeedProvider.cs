using SportSynchro.Application.SportsSeeding.Abstractions;
using SportSynchro.Application.SportsSeeding.Models;
using SportSynchro.Infrastructure.External.TheSportsDb;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Leagues;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Sports;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Teams;

namespace SportSynchro.Application.SportsSeeding;

public sealed class SportsSeedProvider : ISportsSeedProvider
{
    private readonly ITheSportsDbRepository _sportsDb;

    public SportsSeedProvider(ITheSportsDbRepository sportsDb)
    {
        _sportsDb = sportsDb;
    }

    public async Task<IReadOnlyList<SportSeedModel>> LoadSeedDataAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TheSportsDbSportDto> sports =
     await _sportsDb.GetAllSportsAsync(cancellationToken);

        Console.WriteLine($"[DEBUG] Sports from API: {sports.Count}");

        // TEMP: beperk seeding tot 1 sport (sneller testen)
        sports = sports
            .Where(s => string.Equals(
                s.StrSport,
                "Soccer",
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine($"[DEBUG] Sports after filter: {sports.Count}");


        IReadOnlyList<TheSportsDbLeagueDto> allLeagues =
            await _sportsDb.GetAllLeaguesAsync(cancellationToken);

        Console.WriteLine($"[DEBUG] Total leagues from API: {allLeagues.Count}");

        List<SportSeedModel> result = [];

        foreach (TheSportsDbSportDto sport in sports)
        {
            if (!int.TryParse(sport.IdSport, out int sportExternalId))
                continue;

            if (string.IsNullOrWhiteSpace(sport.StrSport))
                continue;

            Console.WriteLine($"[DEBUG] Processing sport: {sport.StrSport}");

            List<TheSportsDbLeagueDto> leaguesForSport =
                allLeagues
                    .Where(l =>
                        string.Equals(
                            l.StrSport,
                            sport.StrSport,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();

            Console.WriteLine(
                $"[DEBUG] Leagues for {sport.StrSport}: {leaguesForSport.Count}");

            List<LeagueSeedModel> leagueModels = [];

            foreach (TheSportsDbLeagueDto league in leaguesForSport)
            {
                if (!int.TryParse(league.IdLeague, out int leagueExternalId))
                    continue;

                if (string.IsNullOrWhiteSpace(league.StrLeague))
                    continue;

                IReadOnlyList<TheSportsDbTeamDto> teams =
                    await _sportsDb.GetTeamsByLeagueAsync(
                        leagueExternalId,
                        cancellationToken);

                List<TeamSeedModel> teamModels =
                    [.. teams
                        .Where(t =>
                            int.TryParse(t.IdTeam, out _) &&
                            !string.IsNullOrWhiteSpace(t.StrTeam) &&
                            !string.IsNullOrWhiteSpace(t.StrCountry))
                        .Select(t => new TeamSeedModel
                        {
                            ExternalId = int.Parse(t.IdTeam!),
                            Name = t.StrTeam!,
                            Country = t.StrCountry!
                        })];

                leagueModels.Add(new LeagueSeedModel
                {
                    ExternalId = leagueExternalId,
                    Name = league.StrLeague!,
                    Teams = teamModels
                });
            }

            result.Add(new SportSeedModel
            {
                ExternalId = sportExternalId,
                Name = sport.StrSport,
                Leagues = leagueModels
            });
        }

        return result;
    }
}
