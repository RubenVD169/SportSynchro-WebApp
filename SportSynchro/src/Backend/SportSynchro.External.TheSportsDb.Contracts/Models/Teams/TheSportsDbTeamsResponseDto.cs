using System.Text.Json.Serialization;

namespace SportSynchro.External.TheSportsDb.Contracts.Models.Teams;

public sealed class TheSportsDbTeamsResponseDto
{
    [JsonPropertyName("list")]
    public List<TheSportsDbTeamDto>? List { get; set; }
}