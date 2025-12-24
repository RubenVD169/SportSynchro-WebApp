using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SportSynchro.Api.Auth;

public sealed class SportSynchroAuthHandler
    : AuthorizationHandler<ClaimOrRoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ClaimOrRoleRequirement requirement)
    {
        // Scope check (OAuth2-compliant)
        bool hasScope = context.User.Claims
            .Where(c => c.Type == "scope")
            .SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Any(s => s == requirement.Scope);

        if (!hasScope)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Client credentials flow => no role check
        bool isUserToken = context.User.HasClaim(c => c.Type == "sub");

        if (!isUserToken)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // No role required
        if (requirement.Role is null)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Role check
        bool hasRole = context.User.Claims.Any(c =>
            c.Type is ClaimTypes.Role or "role"
            && c.Value == requirement.Role);

        if (hasRole)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
