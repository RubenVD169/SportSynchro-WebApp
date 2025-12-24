namespace SportSynchro.External.TheSportsDb.Contracts.Models.Matches;

public sealed class TheSportsDbMatchesResponseDto
{
    public IReadOnlyList<TheSportsDbMatchDto> Schedule { get; init; }
        = [];
}
