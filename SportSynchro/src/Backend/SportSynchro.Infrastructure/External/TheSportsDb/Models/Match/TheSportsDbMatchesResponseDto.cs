namespace SportSynchro.Infrastructure.External.TheSportsDb.Models.Match;

public sealed class TheSportsDbMatchesResponseDto
{
    public IReadOnlyList<TheSportsDbMatchDto> Schedule { get; init; }
        = [];
}
