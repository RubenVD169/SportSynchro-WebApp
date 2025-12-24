using System.Security.Claims;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SportSynchro.Api.Auth;
using SportSynchro.Api.Options;
using SportSynchro.Api.Workers;
using SportSynchro.Application.Interfaces.External;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Services;
using SportSynchro.Infrastructure.External.TheSportsDb;
using SportSynchro.Infrastructure.Options;
using SportSynchro.Infrastructure.Persistence;
using SportSynchro.Infrastructure.Persistence.Seeding;
using SportSynchro.Infrastructure.Persistence.SqlRepositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(nameof(DatabaseOptions)));

builder.Services.Configure<IdentityServerOptions>(
    builder.Configuration.GetSection(nameof(IdentityServerOptions)));

builder.Services.Configure<CorsOptions>(
    builder.Configuration.GetSection(nameof(CorsOptions)));

builder.Services.Configure<TheSportsDbOptions>(
    builder.Configuration.GetSection(nameof(TheSportsDbOptions)));

builder.Services.Configure<SportsSeedingOptions>(
    builder.Configuration.GetSection(nameof(SportsSeedingOptions)));

builder.Services.Configure<TheSportsDbOptions>(
    builder.Configuration.GetSection("TheSportsDb"));

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


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ISportsDbSeeder, SportsDbSeeder>();
builder.Services.AddScoped<ILeagueActivationService, LeagueActivationService>();
builder.Services.AddScoped<ITeamImportService, TeamImportService>();
builder.Services.AddScoped<IMatchImportService, MatchImportService>();
builder.Services.AddScoped<ISportService, SportService>();

builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<ISportRepository, SportRepository>();

// Add authentication and authorization
IdentityServerOptions authOptions = builder.Configuration
    .GetSection(nameof(IdentityServerOptions))
    .Get<IdentityServerOptions>()!;

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5001";
        options.RequireHttpsMetadata = false; // alleen lokaal
        options.Audience = "sportsynchro.api";

        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidAudience = "sportsynchro.api",
            ValidateIssuer = true,
            ValidIssuer = "https://localhost:5001",
            RoleClaimType = "role",
            NameClaimType = "name"
        };
    });



builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Write", policy =>
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
                    scope: "sportsynchro.api.read")));

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

app.MapControllers();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();


app.Run();


