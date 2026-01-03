using System.Text.Json;
using System.Text.Json.Serialization;
using SportSynchro.Application.Interfaces.External;
using SportSynchro.External.TheSportsDb.Contracts.Models.Leagues;
using SportSynchro.External.TheSportsDb.Contracts.Models.Matches;
using SportSynchro.External.TheSportsDb.Contracts.Models.Seasons;
using SportSynchro.External.TheSportsDb.Contracts.Models.Sports;
using SportSynchro.External.TheSportsDb.Contracts.Models.Teams;

namespace SportSynchro.Infrastructure.External.TheSportsDb;

public sealed class TheSportsDbRepository : ITheSportsDbRepository
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

    private readonly HttpClient _httpClient;

    public TheSportsDbRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<TheSportsDbSportDto>> GetAllSportsAsync(
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync("all/sports", cancellationToken);

        response.EnsureSuccessStatusCode();

        await using Stream stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        TheSportsDbSportsResponseDto? dto =
            await JsonSerializer.DeserializeAsync<TheSportsDbSportsResponseDto>(
                stream,
                JsonOptions,
                cancellationToken);

        return dto?.All ?? [];
    }

    public async Task<IReadOnlyList<TheSportsDbLeagueDto>> GetAllLeaguesAsync(
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync("all/leagues", cancellationToken);

        response.EnsureSuccessStatusCode();

        await using Stream stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        TheSportsDbLeaguesResponseDto? dto =
            await JsonSerializer.DeserializeAsync<TheSportsDbLeaguesResponseDto>(
                stream,
                JsonOptions,
                cancellationToken);

        return dto?.All ?? [];
    }

    public async Task<IReadOnlyList<TheSportsDbTeamDto>> GetTeamsByLeagueAsync(
        int leagueExternalId,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync(
                $"list/teams/{leagueExternalId}",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        await using Stream stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        TheSportsDbTeamsResponseDto? dto =
            await JsonSerializer.DeserializeAsync<TheSportsDbTeamsResponseDto>(
                stream,
                JsonOptions,
                cancellationToken);

        return dto?.List ?? [];
    }

    public async Task<IReadOnlyList<string>> GetSeasonsByLeagueAsync(
        int leagueExternalId,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync(
                $"list/seasons/{leagueExternalId}",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        await using Stream stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        TheSportsDbSeasonsResponseDto? dto =
            await JsonSerializer.DeserializeAsync<TheSportsDbSeasonsResponseDto>(
                stream,
                JsonOptions,
                cancellationToken);

        return dto?.List?
            .Where(s => !string.IsNullOrWhiteSpace(s.StrSeason))
            .Select(s => s.StrSeason!)
            .ToList()
            ?? [];
    }

    public async Task<IReadOnlyList<TheSportsDbMatchDto>> GetMatchesByLeagueAndSeasonAsync(
        int leagueExternalId,
        string season,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync(
                $"schedule/league/{leagueExternalId}/{season}",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        await using Stream stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        TheSportsDbMatchesResponseDto? dto =
            await JsonSerializer.DeserializeAsync<TheSportsDbMatchesResponseDto>(
                stream,
                JsonOptions,
                cancellationToken);

        return dto?.Schedule ?? [];
    }
}
