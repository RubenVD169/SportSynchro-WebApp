
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SportSynchro.Api;
using SportSynchro.Api.Auth;
using SportSynchro.Api.Options;
using SportSynchro.Api.Options.ExternalOptions;
using SportSynchro.Api.Workers;
using SportSynchro.Application.Interfaces.External;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Services;
using SportSynchro.Infrastructure.External.TheSportsDb;
using SportSynchro.Infrastructure.Persistence;
using SportSynchro.Infrastructure.Persistence.Seeding;
using SportSynchro.Infrastructure.Persistence.SqlRepositories;
using Stripe;
using SubscriptionService = SportSynchro.Application.Services.SubscriptionService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));

builder.Services.Configure<IdentityServerOptions>(
    builder.Configuration.GetSection(nameof(IdentityServerOptions)));

builder.Services.Configure<CorsOptions>(
    builder.Configuration.GetSection(nameof(CorsOptions)));

builder.Services.Configure<TheSportsDbOptions>(
    builder.Configuration.GetSection(TheSportsDbOptions.SectionName));

builder.Services.Configure<SportsSeedingOptions>(
    builder.Configuration.GetSection(nameof(SportsSeedingOptions)));

builder.Services.Configure<LiveScoreAuthOptions>(
    builder.Configuration.GetSection(LiveScoreAuthOptions.SectionName));

builder.Services.Configure<LiveScoreApiOptions>(
    builder.Configuration.GetSection(LiveScoreApiOptions.SectionName));

//Stripe Configuration
builder.Services.Configure<StripeOptions>(
    builder.Configuration.GetSection(StripeOptions.SectionName));
//stripe services
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddDbContext<SportSynchroDbContext>((sp, options) =>
{
    DatabaseOptions dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    options.UseSqlServer(dbOptions.ConnectionString);
});

builder.Services.AddHttpClient<ITheSportsDbRepository, TheSportsDbRepository>(
    (sp, client) =>
    {
        TheSportsDbOptions options = sp.GetRequiredService<IOptions<TheSportsDbOptions>>().Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.DefaultRequestHeaders.Add("X-API-KEY", options.ApiKey);
    });

builder.Services.AddHttpClient<LiveScoreClient>((sp, client) =>
{
    LiveScoreApiOptions api = sp.GetRequiredService<IOptions<LiveScoreApiOptions>>().Value;
    client.BaseAddress = new Uri(api.BaseUrl);
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ISportsDbSeeder, SportsDbSeeder>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<ITeamImportService, TeamImportService>();
builder.Services.AddScoped<IMatchImportService, MatchImportService>();
builder.Services.AddScoped<ISportService, SportService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IMatchFinalizationService, MatchFinalizationService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<ISportRepository, SportRepository>();
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<ISeasonTeamRepository, SeasonTeamRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

// Add authentication and authorization
IdentityServerOptions authOptions = builder.Configuration
    .GetSection(nameof(IdentityServerOptions))
    .Get<IdentityServerOptions>()!;

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = authOptions.Authority;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.Audience = "sportsynchro.api";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "sportsynchro.api",
            ValidateIssuer = true,
            ValidIssuer = authOptions.Authority,
            RoleClaimType = "role",
            NameClaimType = "name"
        };
    });

builder.Services
    .AddAuthentication("LiveScoreBearer")
    .AddJwtBearer("LiveScoreBearer", options =>
    {
        options.Authority = authOptions.Authority;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidAudience = "sportsynchro.livescore.api",
            ValidateIssuer = true,
            ValidIssuer = authOptions.Authority
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminWrite", policy =>
            policy.Requirements.Add(
                new ClaimOrRoleRequirement(
                    scope: "sportsynchro.api.write",
                    role: "Admin")))
    .AddPolicy("AdminRead", policy =>
            policy.Requirements.Add(
                new ClaimOrRoleRequirement(
                    scope: "sportsynchro.api.read",
                    role: "Admin")))
    .AddPolicy("UserRead", policy =>
            policy.Requirements.Add(
                new ClaimOrRoleRequirement(
                    scope: "sportsynchro.api.read")))
    .AddPolicy("LiveScoreInternal", policy =>
    {
        policy.AddAuthenticationSchemes("LiveScoreBearer");
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "sportsynchro.livescore.read");
    });


builder.Services.AddSingleton<IAuthorizationHandler, SportSynchroAuthHandler>();


CorsOptions corsOptions = builder.Configuration
    .GetSection(nameof(CorsOptions))
    .Get<CorsOptions>()!;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(corsOptions.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<SportsSeedingWorker>();
}

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
