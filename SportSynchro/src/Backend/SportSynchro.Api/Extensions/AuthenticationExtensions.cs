using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SportSynchro.Api.Options;

namespace SportSynchro.Api.Extensions;

public static class AuthenticationExtensions
{
  public static IServiceCollection AddSportSynchroAuthentication(
      this IServiceCollection services,
      IConfiguration configuration,
      IHostEnvironment environment)
  {
    IdentityServerOptions authOptions = configuration
        .GetSection(nameof(IdentityServerOptions))
        .Get<IdentityServerOptions>()!;

    services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("Bearer", options =>
        {
          options.Authority = authOptions.Authority;
          options.RequireHttpsMetadata = !environment.IsDevelopment();
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
        })
        .AddJwtBearer("LiveScoreBearer", options =>
        {
          options.Authority = authOptions.Authority;
          options.RequireHttpsMetadata = false;

          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = authOptions.Authority
          };
        });

    return services;
  }
}
