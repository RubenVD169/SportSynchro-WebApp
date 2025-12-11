using SportSynchro.Domain.Exceptions;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Domain.Entities;

public sealed class Team
{
    private TeamName _name;
    private Team() { } // EF Core only

    public Team(int externalId, TeamName name, string country, int leagueId)
    {
        if (externalId <= 0)
            throw new TeamException("External Team ID must be positive.");

        if (leagueId <= 0)
            throw new TeamException("League ID must be positive.");

        if (string.IsNullOrWhiteSpace(country))
            throw new TeamException("Country is required.");

        ExternalId = externalId;
        _name = name ?? throw new TeamException("Team name is required.");
        Country = country.Trim();
        LeagueId = leagueId;
    }

    public int Id { get; private set; }
    public int ExternalId { get; private set; }
    public int LeagueId { get; private set; }
    public string Country { get; private set; }

    public TeamName Name => _name;
}
