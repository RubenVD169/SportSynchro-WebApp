namespace SportSynchro.LiveScore.Api.Models;

public sealed record LiveMatchResponse(
    string Id,
    string HomeTeam,
    string AwayTeam,
    int HomeScore,
    int AwayScore,
    string? LiveStatus,
    string? Progress
);