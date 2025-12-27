using System.Text.Json.Serialization;

namespace SportSynchro.Api.Contracts.Leagues.Responses;

public sealed record LeagueAdminResponse(
    int Id,
    string Name,
    [property: JsonPropertyName("visible")]
    bool IsVisible
);