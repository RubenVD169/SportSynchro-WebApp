namespace SportSynchro.Application.Models.Leagues;

public sealed record LeagueAdminModel(
    int Id,
    string Name,
    bool IsVisible
);