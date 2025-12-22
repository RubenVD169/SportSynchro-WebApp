using System.Text.Json;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Leagues;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Sports;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Teams;

namespace SportSynchro.Infrastructure.External.TheSportsDb;

public sealed class TheSportsDbRepository : ITheSportsDbRepository
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };
    private readonly HttpClient _httpClient;

    public TheSportsDbRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<TheSportsDbSportDto>> GetAllSportsAsync(
     CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync("all/sports", cancellationToken);
        response.EnsureSuccessStatusCode();

            await using Stream stream =
                await response.Content.ReadAsStreamAsync(cancellationToken);

        TheSportsDbSportsResponseDto? result =
            await JsonSerializer.DeserializeAsync<TheSportsDbSportsResponseDto>(
                stream,
                JsonOptions,
                cancellationToken);

        return result?.All ?? new List<TheSportsDbSportDto>();

    }

    public async Task<IReadOnlyList<TheSportsDbLeagueDto>> GetAllLeaguesAsync(
    CancellationToken cancellationToken)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync("all/leagues", cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        TheSportsDbLeaguesResponseDto? dto =
            await JsonSerializer.DeserializeAsync<
                TheSportsDbLeaguesResponseDto>(
                    stream,
                    JsonOptions,
                    cancellationToken);

        return dto?.All ?? [];
    }

    public async Task<IReadOnlyList<TheSportsDbTeamDto>> GetTeamsByLeagueAsync(
    int leagueExternalId,
    CancellationToken cancellationToken)
    {
        var response =
            await _httpClient.GetAsync(
                $"list/teams/{leagueExternalId}",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        TheSportsDbTeamsResponseDto? dto =
            await JsonSerializer.DeserializeAsync<TheSportsDbTeamsResponseDto>(
                stream,
                JsonOptions,
                cancellationToken);

        return dto?.List ?? [];
    }
}
