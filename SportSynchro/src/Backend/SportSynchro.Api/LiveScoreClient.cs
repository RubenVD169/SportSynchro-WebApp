using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using SportSynchro.Api.Contracts.LiveScore.Responses;
using SportSynchro.Api.Options;
using SportSynchro.Application.Interfaces.Lookups;

namespace SportSynchro.Api;

public sealed class LiveScoreClient
{
    private readonly HttpClient _http;
    private readonly LiveScoreAuthOptions _auth;
    private readonly IHostEnvironment _env;
    private readonly ILeagueExternalIdResolver _leagueResolver;

    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

    public LiveScoreClient(
        HttpClient http,
        IOptions<LiveScoreAuthOptions> auth,
        IHostEnvironment env,
        ILeagueExternalIdResolver leagueResolver)
    {
        _http = http;
        _auth = auth.Value;
        _env = env;
        _leagueResolver = leagueResolver;
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        // Discovery
        DiscoveryDocumentResponse disco = await _http.GetDiscoveryDocumentAsync(
            new DiscoveryDocumentRequest
            {
                Address = _auth.Authority,
                Policy =
                {
                    RequireHttps = !_env.IsDevelopment()
                }
            },
            ct);

        if (disco.IsError)
            throw new InvalidOperationException(disco.Error);

        // Client credentials
        TokenResponse token = await _http.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _auth.ClientId,
                ClientSecret = _auth.ClientSecret,
                Scope = _auth.Scope
            },
            ct);

        return token.IsError ? throw new InvalidOperationException(token.Error) 
            : token.AccessToken ?? throw new InvalidOperationException("No access token received");
    }
    public async Task<IReadOnlyList<LiveMatchResponse>> GetLiveByLeagueAsync(string leagueId, CancellationToken ct)
    {
        // fetch access token
        string accessToken = await GetAccessTokenAsync(ct);

        // apply token
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        string externalLeagueId =
            await _leagueResolver
                .GetExternalLeagueIdAsync(int.Parse(leagueId), ct);

        // call LiveScore endpoint
        HttpResponseMessage response = await _http.GetAsync($"/live/league/{externalLeagueId}", ct);
        response.EnsureSuccessStatusCode();

        await using Stream stream =
            await response.Content.ReadAsStreamAsync(ct);
        
        IReadOnlyList<LiveMatchResponse>? dto =
            await JsonSerializer.DeserializeAsync<List<LiveMatchResponse>>(
                stream,
                JsonOptions,
                ct);

        return dto ?? [];
    }
}