using Microsoft.Extensions.Options;
using SportSynchro.Api.Options;
using SportSynchro.Api.Options.ExternalOptions;
using SportSynchro.Application.Interfaces.External;
using SportSynchro.Infrastructure.External.TheSportsDb;

namespace SportSynchro.Api.Extensions;

public static class ExternalClientsExtensions
{
    public static IServiceCollection AddExternalClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<TheSportsDbOptions>(
            configuration.GetSection(TheSportsDbOptions.SectionName));

        services.Configure<LiveScoreApiOptions>(
            configuration.GetSection(LiveScoreApiOptions.SectionName));

        services.AddHttpClient<ITheSportsDbRepository, TheSportsDbRepository>(
            (sp, client) =>
            {
                TheSportsDbOptions options = sp.GetRequiredService<IOptions<TheSportsDbOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Add("X-API-KEY", options.ApiKey);
            });

        services.AddHttpClient<LiveScoreClient>((sp, client) =>
        {
            LiveScoreApiOptions api = sp.GetRequiredService<IOptions<LiveScoreApiOptions>>().Value;
            client.BaseAddress = new Uri(api.BaseUrl);
        });

        return services;
    }
}
