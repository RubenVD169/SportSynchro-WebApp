using System.Text.Json.Serialization;

namespace SportSynchro.Api.Contracts.Leagues.Requests;

public sealed record SetLeagueVisibilityRequest(
    [property: JsonPropertyName("visible")]
    bool IsVisible
);
