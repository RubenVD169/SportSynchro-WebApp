using SportSynchro.Api.Options;
using SportSynchro.Api.Options.ExternalOptions;
using SportSynchro.Application.Interfaces.Blob;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Services;
using SportSynchro.Application.Services.Blob;
using SportSynchro.Infrastructure.Blob.Options;
using SportSynchro.Infrastructure.Persistence.Seeding;
using Stripe;
using SubscriptionService = SportSynchro.Application.Services.SubscriptionService;

namespace SportSynchro.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SportsSeedingOptions>(
            configuration.GetSection(nameof(SportsSeedingOptions)));

        services.Configure<ScheduleBlobOptions>(
            configuration.GetSection(ScheduleBlobOptions.SectionName));

        services.Configure<StripeOptions>(
            configuration.GetSection(StripeOptions.SectionName));
        
        //stripe service
        services.AddScoped<ProductService>();

        services.AddScoped<ISportsDbSeeder, SportsDbSeeder>();
        services.AddScoped<ILeagueService, LeagueService>();
        services.AddScoped<ITeamImportService, TeamImportService>();
        services.AddScoped<IMatchImportService, MatchImportService>();
        services.AddScoped<ISportService, SportService>();
        services.AddScoped<IMatchService, MatchService>();
        services.AddScoped<IMatchFinalizationService, MatchFinalizationService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ISchedulePdfService, SchedulePdfService>();
        
        return services;
    }
}
