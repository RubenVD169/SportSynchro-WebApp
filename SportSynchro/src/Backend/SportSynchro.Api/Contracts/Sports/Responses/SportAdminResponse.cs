namespace SportSynchro.Api.Contracts.Sports.Responses;

public sealed record SportAdminResponse(
    int Id,
    string Name,
    bool IsVisible
);
