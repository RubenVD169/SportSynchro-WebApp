namespace SportSynchro.Application.Models.Sports;

public sealed record SportAdminModel(
    int Id,
    string Name,
    bool IsVisible
);
