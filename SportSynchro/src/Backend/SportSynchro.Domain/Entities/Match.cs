using SportSynchro.Domain.Exceptions;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Domain.Entities;

public sealed class Match
{
    private MatchStatus _status;

    private Match() { } // EF Core only

    public Match(
        int externalId,
        int seasonId,
        int homeTeamId,
        int awayTeamId,
        DateTime startTimeUtc,
        MatchStatus status,
        int? roundNumber = null,
        int? homeScore = null,
        int? awayScore = null
    )
    {
        if (externalId <= 0)
            throw new MatchException("External match ID must be positive.");

        if (seasonId <= 0)
            throw new MatchException("Season ID must be positive.");

        if (homeTeamId <= 0 || awayTeamId <= 0)
            throw new MatchException("Team IDs must be positive.");

        ExternalId = externalId;
        SeasonId = seasonId;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        StartTimeUtc = startTimeUtc;

        _status = status ?? throw new MatchException("Match status is required.");

        RoundNumber = roundNumber;
        HomeScore = homeScore;
        AwayScore = awayScore;
    }

    public int Id { get; private set; }
    public int ExternalId { get; private set; }

    public int SeasonId { get; private set; }

    public int HomeTeamId { get; private set; }
    public int AwayTeamId { get; private set; }

    public int? RoundNumber { get; private set; }

    public DateTime StartTimeUtc { get; private set; }

    public int? HomeScore { get; private set; }
    public int? AwayScore { get; private set; }

    public MatchStatus Status => _status;

    public void UpdateScore(int? home, int? away)
    {
        HomeScore = home;
        AwayScore = away;
    }

    public void UpdateStatus(MatchStatus status)
        => _status = status ?? throw new MatchException("Match status cannot be null.");
}
