using SportSynchro.Domain.Exceptions;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Domain.Entities;

public sealed class Season
{
    private Season() { } // EF Core only

    public Season(int leagueId, SeasonKey key, bool isCurrent)
    {
        if (leagueId <= 0)
            throw new SeasonException("League ID must be positive.");

        Key = key ?? throw new SeasonException("Season key is required.");

        LeagueId = leagueId;
        IsCurrent = isCurrent;

        TeamsImported = false;
        MatchesImported = false;
        MatchesImportedUntilUtc = null;
    }

    public int Id { get; private set; }
    public int LeagueId { get; private set; }

    public SeasonKey Key { get; private set; }

    public bool IsCurrent { get; private set; }

    public bool TeamsImported { get; private set; }
    public bool MatchesImported { get; private set; }
    public DateTime? MatchesImportedUntilUtc { get; private set; }

    public void MarkTeamsImported()
        => TeamsImported = true;

    public void MarkMatchesImportedUntil(DateTime untilUtc)
    {
        if (untilUtc == default)
            throw new SeasonException("MatchesImportedUntilUtc cannot be default.");

        MatchesImported = true;
        MatchesImportedUntilUtc = untilUtc;
    }

    public void MarkAsCurrent()
        => IsCurrent = true;

    public void MarkAsNotCurrent()
        => IsCurrent = false;
}
