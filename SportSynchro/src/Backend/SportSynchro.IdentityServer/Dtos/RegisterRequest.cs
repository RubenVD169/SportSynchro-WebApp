using System.ComponentModel.DataAnnotations;

namespace SportSynchro.IdentityServer.Dtos;

public sealed class RegisterRequest
{
    [Required]
    public required string Username { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}