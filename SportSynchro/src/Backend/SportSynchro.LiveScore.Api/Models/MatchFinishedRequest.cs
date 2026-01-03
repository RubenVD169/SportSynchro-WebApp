namespace SportSynchro.LiveScore.Api.Models;

public sealed class MatchFinishedRequest
{
    public required string EventId { get; init; }
   public int? HomeScore { get; init; }
    public int? AwayScore { get; init; }
}