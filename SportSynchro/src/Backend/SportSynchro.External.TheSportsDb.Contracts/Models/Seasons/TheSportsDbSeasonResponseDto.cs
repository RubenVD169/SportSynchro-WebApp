namespace SportSynchro.External.TheSportsDb.Contracts.Models.Seasons;

public sealed class TheSportsDbSeasonsResponseDto
{
    public IReadOnlyList<TheSportsDbSeasonDto>? List { get; init; } = [];
}