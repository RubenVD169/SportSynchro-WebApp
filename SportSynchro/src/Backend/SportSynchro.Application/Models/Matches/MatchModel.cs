namespace SportSynchro.Application.Models.Matches;

public sealed record MatchModel(
    int Id,
    string LeagueName,
    DateTime MatchDate,
    string HomeTeam,
    string AwayTeam,
    int HomeScore,
    int AwayScore,
    string? Status
);