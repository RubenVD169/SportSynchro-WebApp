namespace SportSynchro.Infrastructure.External.TheSportsDb.Models.Match;

public sealed class TheSportsDbMatchDto
{
    public int IdEvent { get; init; }
    public int IdLeague { get; init; }
    public int IdHomeTeam { get; init; }
    public int IdAwayTeam { get; init; }
    public string StrTimestamp { get; init; } = default!;
    public int? IntRound { get; init; }
    public int? IntHomeScore { get; init; }
    public int? IntAwayScore { get; init; }
    public string StrStatus { get; init; } = default!;
}
