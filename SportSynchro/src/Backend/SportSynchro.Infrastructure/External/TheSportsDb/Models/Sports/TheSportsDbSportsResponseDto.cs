using System.Text.Json.Serialization;

namespace SportSynchro.Infrastructure.External.TheSportsDb.Models.Sports;

public sealed class TheSportsDbSportsResponseDto
{
    [JsonPropertyName("all")]
    public List<TheSportsDbSportDto>? All { get; set; }
}
