using SportSynchro.LiveScore.Api.Infrastructure;
using SportSynchro.LiveScore.Api.Models;

namespace SportSynchro.LiveScore.Api.Endpoints;

public static class LiveScoreEndpoints
{
    public static void MapLiveScoreEndpoints(this WebApplication app)
    {
        app.MapGet("/live/league/{leagueId}", async (
            string leagueId,
            ILiveMatchRepository repository,
            CancellationToken ct) =>
        {
            IReadOnlyList<LiveMatchDocument> matches = await repository.GetByLeagueAsync(leagueId, ct);
            IReadOnlyList<LiveMatchResponse> response = MapToResponseList(matches);
            return Results.Ok(response);
        });

        app.MapGet("/live/event/{leagueId}/{eventId}", async (
            string eventId,
            string leagueId,
            ILiveMatchRepository repository,
            CancellationToken ct) =>
        {
            LiveMatchDocument? match = await repository.GetByEventAsync(eventId, leagueId, ct);

            return match is null
                ? Results.NotFound()
                : Results.Ok(match);
        });
    }

    private static LiveMatchResponse MapToResponse(
    LiveMatchDocument live)
    {
        return new LiveMatchResponse(
            live.Id,
            live.HomeTeamName,
            live.AwayTeamName,
            int.TryParse(live.HomeScore, out int hs) ? hs : 0,
            int.TryParse(live.AwayScore, out int aw) ? aw : 0,
            live.Status,
            live.Progress);
    }

    private static IReadOnlyList<LiveMatchResponse> MapToResponseList(
        IReadOnlyList<LiveMatchDocument> liveMatches)
    {
        return [.. liveMatches.Select(MapToResponse)];
    }

}
