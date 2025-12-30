using System.Text.Json;
using System.Text.Json.Serialization;
using SportSynchro.LiveScore.Api.Application;

namespace SportSynchro.LiveScore.Api.Infrastructure;

public sealed class TheSportsDbLiveScoreClient
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

    private readonly HttpClient _httpClient;

    public TheSportsDbLiveScoreClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<SportsDbLiveScoreInput>> GetAllLiveScoresAsync(
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync("livescore/all", cancellationToken);

        response.EnsureSuccessStatusCode();

        await using Stream stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        LiveScoreResponseDto? dto =
            await JsonSerializer.DeserializeAsync<LiveScoreResponseDto>(
                stream,
                JsonOptions,
                cancellationToken);

        return dto?.LiveScore ?? [];
    }

    private sealed class LiveScoreResponseDto
    {
        [JsonPropertyName("livescore")]
        public List<SportsDbLiveScoreInput>? LiveScore { get; init; }
    }
}
