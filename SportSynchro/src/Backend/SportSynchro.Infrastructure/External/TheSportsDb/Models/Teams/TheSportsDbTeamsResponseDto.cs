using System.Text.Json.Serialization;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Teams;

public sealed class TheSportsDbTeamsResponseDto
{
    [JsonPropertyName("list")]
    public List<TheSportsDbTeamDto>? List { get; set; }
}
