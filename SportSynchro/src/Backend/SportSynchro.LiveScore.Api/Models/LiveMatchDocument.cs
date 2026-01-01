using System.Text.Json.Serialization;
using SportSynchro.LiveScore.Api.Application;

namespace SportSynchro.LiveScore.Api.Models;

public sealed record LiveMatchDocument
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("idEvent")]
    public required string IdEvent { get; init; }

    [JsonPropertyName("idLeague")]
    public required string IdLeague { get; init; }

    [JsonPropertyName("idLiveScore")]
    public string? IdLiveScore { get; init; }

    [JsonPropertyName("idHomeTeam")]
    public required string HomeTeamId { get; init; }

    [JsonPropertyName("strHomeTeam")]
    public required string HomeTeamName { get; init; }

    [JsonPropertyName("idAwayTeam")]
    public required string AwayTeamId { get; init; }

    [JsonPropertyName("strAwayTeam")]
    public required string AwayTeamName { get; init; }

    [JsonPropertyName("intHomeScore")]
    public string? HomeScore { get; init; }

    [JsonPropertyName("intAwayScore")]
    public string? AwayScore { get; init; }

    [JsonPropertyName("strStatus")]
    public string? Status { get; init; }

    [JsonPropertyName("strProgress")]
    public string? Progress { get; init; }

    [JsonPropertyName("updated")]
    public string? UpdatedRaw { get; init; }

    [JsonPropertyName("ttl")]
    public int? TimeToLiveSeconds { get; set; }

    [JsonPropertyName("finishedNotified")]
    public bool FinishedNotified { get; set; } = false;

    public static LiveMatchDocument CreateFromSportsDb(
        SportsDbLiveScoreInput input)
    {
        return new LiveMatchDocument
        {
            Id = $"event-{input.IdEvent}",
            IdEvent = input.IdEvent,
            IdLeague = input.IdLeague,
            IdLiveScore = input.IdLiveScore,

            HomeTeamId = input.IdHomeTeam,
            HomeTeamName = input.StrHomeTeam,
            AwayTeamId = input.IdAwayTeam,
            AwayTeamName = input.StrAwayTeam,

            HomeScore = input.IntHomeScore,
            AwayScore = input.IntAwayScore,
            Status = input.StrStatus,
            Progress = input.StrProgress,
            UpdatedRaw = input.Updated
        };
    }
    public LiveMatchDocument WithUpdatedSnapshot(
        SportsDbLiveScoreInput input,
        int? ttl)
    {
        return this with
        {
            HomeScore = input.IntHomeScore,
            AwayScore = input.IntAwayScore,
            Status = input.StrStatus,
            Progress = input.StrProgress,
            UpdatedRaw = input.Updated,
            TimeToLiveSeconds = ttl
        };
    }
}
