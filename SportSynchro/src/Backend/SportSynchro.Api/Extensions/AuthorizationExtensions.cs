using Microsoft.AspNetCore.Authorization;
using SportSynchro.Api.Auth;

namespace SportSynchro.Api.Extensions;

public static class AuthorizationExtensions
{
  public static IServiceCollection AddSportSynchroAuthorization(
      this IServiceCollection services)
  {
    services.AddAuthorizationBuilder()
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

    services.AddSingleton<IAuthorizationHandler, SportSynchroAuthHandler>();

    return services;
  }
}
