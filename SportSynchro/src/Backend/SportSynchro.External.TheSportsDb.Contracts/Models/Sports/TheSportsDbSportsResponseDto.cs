using System.Text.Json.Serialization;

namespace SportSynchro.External.TheSportsDb.Contracts.Models.Sports;

public sealed class TheSportsDbSportsResponseDto
{
    [JsonPropertyName("all")]
    public List<TheSportsDbSportDto>? All { get; set; }
}
