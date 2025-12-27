namespace SportSynchro.Api.Contracts.Sports.Requests;

using System.Text.Json.Serialization;

public sealed record SetSportVisibilityRequest(
    [property: JsonPropertyName("visible")]
    bool IsVisible
);

