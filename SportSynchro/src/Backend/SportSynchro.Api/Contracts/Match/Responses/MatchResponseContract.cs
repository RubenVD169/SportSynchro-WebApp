namespace SportSynchro.Api.Contracts.Match.Responses;

public sealed record MatchResponseContract(
    int Id,
    string LeagueName,
    DateTime MatchDate,
    string HomeTeam,
    string AwayTeam,
    int HomeScore = 0,
    int AwayScore = 0   
);