using System.Net.Http.Headers;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using SportSynchro.LiveScore.Api.Models;
using SportSynchro.LiveScore.Api.Options;

namespace SportSynchro.LiveScore.Api.Infrastructure;

public sealed class SportSynchroApiClient
{
    private readonly HttpClient _http;
    private readonly SportSynchroApiAuthOptions _auth;

    public SportSynchroApiClient(
        HttpClient http,
        IOptions<SportSynchroApiAuthOptions> auth)
    {
        _http = http;
        _auth = auth.Value;
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        DiscoveryDocumentResponse disco = await _http.GetDiscoveryDocumentAsync(
            new DiscoveryDocumentRequest
            {
                Address = _auth.Authority,
                Policy = { RequireHttps = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development" }
            },
            ct);

        if (disco.IsError)
            throw new InvalidOperationException(disco.Error);

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
            : token.AccessToken ?? throw new InvalidOperationException("No access token received.");
    }

    public async Task NotifyMatchFinishedAsync(
        MatchFinishedRequest request,
        CancellationToken ct)
    {
        string accessToken = await GetAccessTokenAsync(ct);

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await _http.PostAsJsonAsync(
            "/internal/matches/finished",
            request,
            ct);

        response.EnsureSuccessStatusCode();
    }
}