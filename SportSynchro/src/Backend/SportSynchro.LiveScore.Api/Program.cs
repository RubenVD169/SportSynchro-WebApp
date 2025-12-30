using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddScoped<LiveScorePollerService>();

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

IdentityServerOptions authOptions = builder.Configuration
    .GetSection(nameof(IdentityServerOptions))
    .Get<IdentityServerOptions>()!;

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = authOptions.Authority;
        options.RequireHttpsMetadata = builder.Environment.IsDevelopment() == false; 

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = authOptions.Audience,

            ValidateIssuer = true,
            ValidIssuer = authOptions.Authority
        };
    });


builder.Services.AddAuthorizationBuilder()
    .AddPolicy("LiveScoreRead", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "sportsynchro.livescore.read");
    });

builder.Services.AddHttpClient<SportSynchroApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["SportSynchroApiClient:BaseUrl"]!);
});

WebApplication app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapLiveScoreEndpoints();
app.MapLiveScoreIngestEndpoints();

app.Run();
