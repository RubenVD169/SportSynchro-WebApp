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
            return Results.Ok(matches);
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
}
