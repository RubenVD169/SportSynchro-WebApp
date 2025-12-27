using SportSynchro.Domain.Exceptions;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Domain.Entities;

public sealed class League
{
    private LeagueName _name;

    private League() { } // EF Core only

    public League(int externalId, LeagueName name, int sportId, bool isVisible)
    {
        if (externalId <= 0)
            throw new LeagueException("External League ID must be positive.");

        if (sportId <= 0)
            throw new LeagueException("Sport ID must be positive.");

        _name = name ?? throw new LeagueException("League name is required.");

        ExternalId = externalId;
        SportId = sportId;
        IsVisible = isVisible;
        TeamsImported = false;
        MatchesImported = false;
        MatchesImportedUntilUtc = null;
    }

    public int Id { get; private set; }
    public int ExternalId { get; private set; }
    public int SportId { get; private set; }
    public bool IsVisible { get; private set; }
    public bool TeamsImported { get; private set; }
    public bool MatchesImported { get; private set; }
    public DateTime? MatchesImportedUntilUtc { get; private set; }

    public LeagueName Name => _name;
    
    public void SetVisibility(bool visible)
        => IsVisible = visible;
    
    public void MarkTeamsImported()
        => TeamsImported = true;

    public void MarkMatchesImportedUntil(DateTime untilUtc)
    {
        if (untilUtc == default)
            throw new LeagueException("MatchesImportedUntilUtc cannot be default.");

        MatchesImported = true;
        MatchesImportedUntilUtc = untilUtc;
    }
}
