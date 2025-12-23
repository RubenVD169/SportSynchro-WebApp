using SportSynchro.Application.SportsSeeding.Abstractions;
using SportSynchro.Application.SportsSeeding.Models;
using SportSynchro.Infrastructure.External.TheSportsDb;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Leagues;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Sports;

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

        IReadOnlyList<TheSportsDbLeagueDto> leagues =
            await _sportsDb.GetAllLeaguesAsync(cancellationToken);

        List<SportSeedModel> result = [];

        foreach (TheSportsDbSportDto sport in sports)
        {
            if (!int.TryParse(sport.IdSport, out int sportExternalId))
                continue;

            if (string.IsNullOrWhiteSpace(sport.StrSport))
                continue;

            List<LeagueSeedModel> leaguesForSport =
                [.. leagues
                    .Where(l =>
                        string.Equals(
                            l.StrSport,
                            sport.StrSport,
                            StringComparison.OrdinalIgnoreCase))
                    .Select(l => new LeagueSeedModel
                    {
                        ExternalId = int.Parse(l.IdLeague!),
                        Name = l.StrLeague!
                    })];

            if (leaguesForSport.Count == 0)
                continue;

            result.Add(new SportSeedModel
            {
                ExternalId = sportExternalId,
                Name = sport.StrSport,
                Leagues = leaguesForSport
            });
        }

        return result;
    }
}
