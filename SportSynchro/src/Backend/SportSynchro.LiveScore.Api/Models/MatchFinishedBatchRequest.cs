namespace SportSynchro.LiveScore.Api.Models;

public sealed record MatchFinishedBatchRequest(
    IReadOnlyList<MatchFinishedRequest> Matches
);
