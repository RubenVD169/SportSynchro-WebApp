using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.SportsSeeding.Abstractions;
using SportSynchro.Application.SportsSeeding.Models;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;
using SportSynchro.Infrastructure.Persistence;

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
        IReadOnlyList<SportSeedModel> seedData = await _seedProvider.LoadSeedDataAsync(cancellationToken);

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
        foreach (var leagueSeed in leagues)
        {
            League? league =
                await _db.Leagues
                    .FirstOrDefaultAsync(
                        l => l.ExternalId == leagueSeed.ExternalId,
                        ct);

            if (league is null)
            {
                league = new League(
                    leagueSeed.ExternalId,
                    LeagueName.Create(leagueSeed.Name),
                    sport.Id,
                    isVisible: false);

                _db.Leagues.Add(league);
                await _db.SaveChangesAsync(ct);
            }

            await SeedTeamsAsync(league, leagueSeed.Teams, ct);
        }
    }

    private async Task SeedTeamsAsync(
    League league,
    IReadOnlyList<TeamSeedModel> teams,
    CancellationToken ct)
    {
        foreach (TeamSeedModel teamSeed in teams)
        {
            bool exists =
                await _db.Teams.AnyAsync(
                    t => t.ExternalId == teamSeed.ExternalId,
                    ct);

            if (exists)
                continue;

            Team team = new(
                teamSeed.ExternalId,
                TeamName.Create(teamSeed.Name),
                teamSeed.Country,
                league.Id);

            _db.Teams.Add(team);
        }

        await _db.SaveChangesAsync(ct);
    }


}
