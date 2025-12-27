using System.Text.Json.Serialization;

namespace SportSynchro.Api.Contracts.Sports.Responses;

public sealed record SportAdminResponse(
    int Id,
    string Name,
    [property: JsonPropertyName("visible")]
    bool IsVisible
);
