namespace SportSynchro.Api.Contracts.LiveScore.Requests;

public sealed class MatchFinishedRequest
{
    public required string EventId { get; init; }
    public required string LeagueId { get; init; }
    public required string HomeTeamId { get; init; }
    public required string AwayTeamId { get; init; }

    public int? HomeScore { get; init; }
    public int? AwayScore { get; init; }

    public DateTime FinishedAtUtc { get; init; }
}
