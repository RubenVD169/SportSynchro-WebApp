using System.Text.Json.Serialization;

namespace SportSynchro.External.TheSportsDb.Contracts.Models.Leagues;

public sealed class TheSportsDbLeaguesResponseDto
{
    [JsonPropertyName("all")]
    public List<TheSportsDbLeagueDto>? All { get; set; }
}