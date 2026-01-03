using SportSynchro.Api.Contracts.Sports.Responses;
using SportSynchro.Application.Models.Sports;

namespace SportSynchro.Api.Mapping.Sports;

public static class SportMappingExtensions
{
    public static SportAdminResponse ToAdminResponse(
        this SportAdminModel model)
    {
        return new SportAdminResponse(
            model.Id,
            model.Name,
            model.IsVisible
        );
    }

    public static IReadOnlyList<SportAdminResponse> ToAdminResponses(
        this IEnumerable<SportAdminModel> models)
    {
        return [.. models.Select(ToAdminResponse)];
    }

    public static SportUserResponse ToUserResponse(
        this SportUserModel model)
    {
        return new SportUserResponse(
            model.Id,
            model.Name
        );
    }

    public static IReadOnlyList<SportUserResponse> ToUserResponses(
        this IEnumerable<SportUserModel> models)
    {
        return [.. models.Select(ToUserResponse)];
    }
}