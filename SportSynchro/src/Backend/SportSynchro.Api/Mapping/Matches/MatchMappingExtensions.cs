using SportSynchro.Api.Contracts.Match.Responses;
using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Api.Mapping.Matches;

public static class MatchMappingExtensions
{
    public static MatchResponseContract 
        ToResponseContract(this MatchModel model)
    {
        return new MatchResponseContract(
            model.Id,
            model.LeagueName,
            model.MatchDate,
            model.HomeTeam,
            model.AwayTeam,
            model.HomeScore,
            model.AwayScore
        );
    }

    public static IReadOnlyList<MatchResponseContract> 
        ToResponseContractList(this IReadOnlyList<MatchModel> models)
    {
        return [.. models.Select(m => m.ToResponseContract())];
    }
}