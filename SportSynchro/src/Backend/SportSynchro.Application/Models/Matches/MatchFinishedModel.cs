namespace SportSynchro.Application.Models.Matches;

public sealed record MatchFinishedModel(
    string EventId,
    string LeagueId,
    string HomeTeamId,
    string AwayTeamId,
    int? HomeScore,
    int? AwayScore,
    DateTime FinishedAtUtc
);