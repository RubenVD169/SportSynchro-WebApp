using SportSynchro.Api.Contracts.Leagues.Responses;
using SportSynchro.Application.Models.Leagues;

namespace SportSynchro.Api.Mapping.Leagues;

public static class LeagueMappingExtensions
{
    public static LeagueAdminResponse ToAdminResponse(
        this LeagueAdminModel model)
    {
        return new LeagueAdminResponse(
            model.Id,
            model.Name,
            model.IsVisible
        );
    }

    public static IReadOnlyList<LeagueAdminResponse> ToAdminResponses(
        this IEnumerable<LeagueAdminModel> models)
    {
        return [.. models.Select(ToAdminResponse)];
    }
}