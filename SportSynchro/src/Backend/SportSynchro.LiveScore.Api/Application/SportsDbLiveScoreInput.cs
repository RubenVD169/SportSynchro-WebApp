using Newtonsoft.Json;

namespace SportSynchro.LiveScore.Api.Application;

public sealed class SportsDbLiveScoreInput
{
    [JsonProperty("idLiveScore")]
    public string? IdLiveScore { get; init; }

    [JsonProperty("idEvent")]
    public required string IdEvent { get; init; }

    [JsonProperty("strSport")]
    public string? StrSport { get; init; }

    [JsonProperty("idLeague")]
    public required string IdLeague { get; init; }

    [JsonProperty("strLeague")]
    public string? StrLeague { get; init; }

    [JsonProperty("idHomeTeam")]
    public required string IdHomeTeam { get; init; }

    [JsonProperty("idAwayTeam")]
    public required string IdAwayTeam { get; init; }

    [JsonProperty("strHomeTeam")]
    public required string StrHomeTeam { get; init; }

    [JsonProperty("strAwayTeam")]
    public required string StrAwayTeam { get; init; }

    [JsonProperty("strHomeTeamBadge")]
    public string? StrHomeTeamBadge { get; init; }

    [JsonProperty("strAwayTeamBadge")]
    public string? StrAwayTeamBadge { get; init; }

    [JsonProperty("intHomeScore")]
    public string? IntHomeScore { get; init; }

    [JsonProperty("intAwayScore")]
    public string? IntAwayScore { get; init; }

    [JsonProperty("intEventScore")]
    public string? IntEventScore { get; init; }

    [JsonProperty("intEventScoreTotal")]
    public string? IntEventScoreTotal { get; init; }

    [JsonProperty("strStatus")]
    public string? StrStatus { get; init; }

    [JsonProperty("strProgress")]
    public string? StrProgress { get; init; }

    [JsonProperty("strEventTime")]
    public string? StrEventTime { get; init; }

    [JsonProperty("dateEvent")]
    public string? DateEvent { get; init; }

    [JsonProperty("updated")]
    public string? Updated { get; init; }
}
