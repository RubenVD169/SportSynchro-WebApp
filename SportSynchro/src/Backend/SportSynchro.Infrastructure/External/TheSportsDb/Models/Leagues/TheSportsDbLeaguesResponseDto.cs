using System.Text.Json.Serialization;

namespace SportSynchro.Infrastructure.External.TheSportsDb.Models.Leagues;

public sealed class TheSportsDbLeaguesResponseDto
{
    [JsonPropertyName("all")]
    public List<TheSportsDbLeagueDto>? All { get; set; }
}