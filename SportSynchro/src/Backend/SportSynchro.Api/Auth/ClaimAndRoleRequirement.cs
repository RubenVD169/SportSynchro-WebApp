using Microsoft.AspNetCore.Authorization;

namespace SportSynchro.Api.Auth;

public sealed class ClaimOrRoleRequirement : IAuthorizationRequirement
{
    public string Scope { get; }
    public string? Role { get; }

    public ClaimOrRoleRequirement(string scope, string? role = null)
    {
        Scope = scope;
        Role = role;
    }
}