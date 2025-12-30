using System.Text.Json.Serialization;

namespace SportSynchro.LiveScore.Api.Models;

public sealed class LiveMatchDocument
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("idEvent")]
    public required string IdEvent { get; init; }

    [JsonPropertyName("idLeague")]
    public required string IdLeague { get; init; }

    [JsonPropertyName("idLiveScore")]
    public string? IdLiveScore { get; init; }

    [JsonPropertyName("strSport")]
    public string? Sport { get; init; }

    [JsonPropertyName("strLeague")]
    public string? LeagueName { get; init; }

    [JsonPropertyName("idHomeTeam")]
    public required string HomeTeamId { get; init; }

    [JsonPropertyName("strHomeTeam")]
    public required string HomeTeamName { get; init; }

    [JsonPropertyName("strHomeTeamBadge")]
    public string? HomeTeamBadge { get; init; }

    [JsonPropertyName("idAwayTeam")]
    public required string AwayTeamId { get; init; }

    [JsonPropertyName("strAwayTeam")]
    public required string AwayTeamName { get; init; }

    [JsonPropertyName("strAwayTeamBadge")]
    public string? AwayTeamBadge { get; init; }

    [JsonPropertyName("intHomeScore")]
    public string? HomeScore { get; init; }

    [JsonPropertyName("intAwayScore")]
    public string? AwayScore { get; init; }

    [JsonPropertyName("intEventScore")]
    public string? EventScore { get; init; }

    [JsonPropertyName("intEventScoreTotal")]
    public string? EventScoreTotal { get; init; }

    [JsonPropertyName("strStatus")]
    public string? Status { get; init; }

    [JsonPropertyName("strProgress")]
    public string? Progress { get; init; }

    [JsonPropertyName("strEventTime")]
    public string? EventTime { get; init; }

    [JsonPropertyName("dateEvent")]
    public string? DateEvent { get; init; }

    [JsonPropertyName("updated")]
    public string? UpdatedRaw { get; init; }

    [JsonPropertyName("ttl")]
    public int? TimeToLiveSeconds { get; init; }

    [JsonPropertyName("finishedNotified")]
    public bool FinishedNotified { get; set; } = false;
}
