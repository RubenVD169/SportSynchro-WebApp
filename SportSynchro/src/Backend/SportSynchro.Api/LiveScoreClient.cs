using System.Net.Http.Headers;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using SportSynchro.Api.Options;

namespace SportSynchro.Api;

public sealed class LiveScoreClient
{
    private readonly HttpClient _http;
    private readonly LiveScoreAuthOptions _auth;
    private readonly IHostEnvironment _env;

    public LiveScoreClient(
        HttpClient http,
        IOptions<LiveScoreAuthOptions> auth,
        IHostEnvironment env)
    {
        _http = http;
        _auth = auth.Value;
        _env = env;
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
    public async Task<string> GetLiveByLeagueAsync(string leagueId, CancellationToken ct)
    {
        // fetch access token
        string accessToken = await GetAccessTokenAsync(ct);

        // apply token
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        // call LiveScore endpoint
        HttpResponseMessage response = await _http.GetAsync($"/live/league/{leagueId}", ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(ct);
    }
}