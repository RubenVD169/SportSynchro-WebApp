using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using SportSynchro.LiveScore.Api.Application;
using SportSynchro.LiveScore.Api.Endpoints;
using SportSynchro.LiveScore.Api.Infrastructure;
using SportSynchro.LiveScore.Api.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CosmosOptions>(
    builder.Configuration.GetSection(CosmosOptions.SectionName));

builder.Services.Configure<TheSportsDbOptions>(
    builder.Configuration.GetSection(TheSportsDbOptions.SectionName));

builder.Services.Configure<SportSynchroApiAuthOptions>(
    builder.Configuration.GetSection(SportSynchroApiAuthOptions.SectionName));

builder.Services.Configure<SportSynchroApiClientOptions>(
    builder.Configuration.GetSection(SportSynchroApiClientOptions.SectionName));

builder.Services.AddScoped<ILiveMatchRepository, CosmosLiveMatchRepository>();
builder.Services.AddScoped<LiveScoreIngestService>();

builder.Services.AddHttpClient<SportSynchroApiClient>(client =>
{
    SportSynchroApiClientOptions options =
        builder.Configuration
            .GetSection(SportSynchroApiClientOptions.SectionName)
            .Get<SportSynchroApiClientOptions>()!;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddHttpClient<TheSportsDbLiveScoreClient>(
    (sp, client) =>
    {
        TheSportsDbOptions options =
            sp.GetRequiredService<IOptions<TheSportsDbOptions>>().Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.DefaultRequestHeaders.Add("X-API-KEY", options.ApiKey);
    });

builder.Services.AddHostedService<LiveScorePollerService>();

WebApplication app = builder.Build();

app.MapLiveScoreEndpoints();
app.MapLiveScoreIngestEndpoints();

app.Run();
