using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SportSynchro.Api.Options;
using SportSynchro.Application.Interfaces.Blob;
using SportSynchro.Application.Interfaces.Lookups;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Infrastructure.Blob;
using SportSynchro.Infrastructure.Caching;
using SportSynchro.Infrastructure.Persistence;
using SportSynchro.Infrastructure.Persistence.SqlRepositories;

namespace SportSynchro.Api.Extensions;

public static class PersistenceExtensions
{
  public static IServiceCollection AddPersistence(
      this IServiceCollection services,
      IConfiguration configuration)
  {
    services.Configure<DatabaseOptions>(
        configuration.GetSection(DatabaseOptions.SectionName));

    services.AddDbContext<SportSynchroDbContext>((sp, options) =>
    {
      DatabaseOptions dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
      options.UseSqlServer(dbOptions.ConnectionString);
    });

    services.AddMemoryCache();

    services.AddScoped<ILeagueRepository, LeagueRepository>();
    services.AddScoped<ITeamRepository, TeamRepository>();
    services.AddScoped<IMatchRepository, MatchRepository>();
    services.AddScoped<ISportRepository, SportRepository>();
    services.AddScoped<ISeasonRepository, SeasonRepository>();
    services.AddScoped<ISeasonTeamRepository, SeasonTeamRepository>();
    services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
    services.AddScoped<ISchedulePdfRepository, SchedulePdfRepository>();
    
    services.AddScoped<ILeagueExternalIdResolver, LeagueExternalIdResolver>();
    services.AddScoped<ISeasonResolver, SeasonResolver>();

    return services;
  }
}
