using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.SportsSeeding.Abstractions;
using SportSynchro.Application.SportsSeeding.Models;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;
using SportSynchro.Infrastructure.Persistence;

namespace SportSynchro.Application.SportsSeeding;

public sealed class SportsDbSeeder : ISportsDbSeeder
{
    private readonly ISportsSeedProvider _seedProvider;
    private readonly SportSynchroDbContext _db;

    public SportsDbSeeder(
        ISportsSeedProvider seedProvider,
        SportSynchroDbContext db)
    {
        _seedProvider = seedProvider;
        _db = db;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SportSeedModel> seedData =
            await _seedProvider.LoadSeedDataAsync(cancellationToken);

        foreach (SportSeedModel sportSeed in seedData)
        {
            Sport? sport =
                await _db.Sports
                    .FirstOrDefaultAsync(
                        s => s.ExternalId == sportSeed.ExternalId,
                        cancellationToken);

            if (sport is null)
            {
                sport = new Sport(
                    sportSeed.ExternalId,
                    SportName.Create(sportSeed.Name),
                    isVisible: false);

                _db.Sports.Add(sport);
                await _db.SaveChangesAsync(cancellationToken);
            }

            await SeedLeaguesAsync(sport, sportSeed.Leagues, cancellationToken);
        }
    }

    private async Task SeedLeaguesAsync(
        Sport sport,
        IReadOnlyList<LeagueSeedModel> leagues,
        CancellationToken ct)
    {
        // Get existing leagues for the sport
        Dictionary<int, League> existingLeagues =
            await _db.Leagues
                .Where(l => l.SportId == sport.Id)
                .ToDictionaryAsync(l => l.ExternalId, ct);

        List<League> newLeagues = [];

        foreach (LeagueSeedModel leagueSeed in leagues)
        {
            if (existingLeagues.ContainsKey(leagueSeed.ExternalId))
                continue;

            League league = new(
                leagueSeed.ExternalId,
                LeagueName.Create(leagueSeed.Name),
                sport.Id,
                isVisible: false);

            newLeagues.Add(league);
            existingLeagues.Add(leagueSeed.ExternalId, league);
        }

        if (newLeagues.Count > 0)
        {
            _db.Leagues.AddRange(newLeagues);
            await _db.SaveChangesAsync(ct);
        }
    }
}