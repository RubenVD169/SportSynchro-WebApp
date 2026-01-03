using SportSynchro.Domain.Exceptions;

public sealed class SeasonTeam
{
    private SeasonTeam() { } // EF Core only

    public SeasonTeam(int seasonId, int teamId)
    {
        if (seasonId <= 0 || teamId <= 0)
            throw new SeasonTeamException("Ids must be positive.");

        SeasonId = seasonId;
        TeamId = teamId;
    }

    public int SeasonId { get; private set; }
    public int TeamId { get; private set; }
}
