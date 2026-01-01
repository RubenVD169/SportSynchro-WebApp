namespace SportSynchro.Application.Models.Matches;

public sealed record MatchFinishedModel(
    string EventId,
    int? HomeScore,
    int? AwayScore
);