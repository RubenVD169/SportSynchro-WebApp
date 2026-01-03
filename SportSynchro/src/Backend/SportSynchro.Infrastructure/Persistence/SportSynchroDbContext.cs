using Microsoft.EntityFrameworkCore;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence;

public sealed class SportSynchroDbContext(DbContextOptions<SportSynchroDbContext> options) : DbContext(options)
{
    public DbSet<League> Leagues => Set<League>();
    public DbSet<Sport> Sports => Set<Sport>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<SeasonTeam> SeasonTeams => Set<SeasonTeam>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SportSynchroDbContext).Assembly);
    }
}