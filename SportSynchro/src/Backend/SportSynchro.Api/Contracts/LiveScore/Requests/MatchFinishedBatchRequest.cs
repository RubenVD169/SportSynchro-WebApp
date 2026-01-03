namespace SportSynchro.Api.Contracts.LiveScore.Requests;

public sealed record MatchFinishedBatchRequest(
    IReadOnlyList<MatchFinishedRequest> Matches
);
