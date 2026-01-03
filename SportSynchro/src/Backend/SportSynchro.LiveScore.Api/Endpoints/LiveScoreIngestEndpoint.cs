using SportSynchro.LiveScore.Api.Application;

namespace SportSynchro.LiveScore.Api.Endpoints;

public static class LiveScoreIngestEndpoints
{
    public static void MapLiveScoreIngestEndpoints(this WebApplication app)
    {
        app.MapPost("/internal/livescore", async (
            SportsDbLiveScoreInput input,
            LiveScoreIngestService ingestService,
            CancellationToken ct) =>
        {
            await ingestService.UpsertFromSportsDbAsync([input], ct);
            return Results.Accepted();
        })
        .RequireAuthorization("LiveScoreRead");
    }
}
