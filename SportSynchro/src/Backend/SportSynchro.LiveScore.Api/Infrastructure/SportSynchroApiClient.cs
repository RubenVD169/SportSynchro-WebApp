using System.Net.Http.Headers;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using SportSynchro.LiveScore.Api.Models;
using SportSynchro.LiveScore.Api.Options;

namespace SportSynchro.LiveScore.Api.Infrastructure;
/* A client for interacting with the SportSynchro API, handling authentication
   and providing methods to notify about match events. With the least amount of db calls. */
public sealed class SportSynchroApiClient
{
    private readonly HttpClient _http;
    private readonly SportSynchroApiAuthOptions _auth;

    private string? _cachedAccessToken;
    private DateTimeOffset _accessTokenExpiresAt;

    public SportSynchroApiClient(
        HttpClient http,
        IOptions<SportSynchroApiAuthOptions> auth)
    {
        _http = http;
        _auth = auth.Value;
    }

    //Token Handeling
    private async Task<string> GetValidAccessTokenAsync(CancellationToken ct)
    {
        if (_cachedAccessToken is not null &&
            _accessTokenExpiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
        {
            return _cachedAccessToken;
        }

        DiscoveryDocumentResponse disco =
            await _http.GetDiscoveryDocumentAsync(
                new DiscoveryDocumentRequest
                {
                    Address = _auth.Authority,
                    Policy =
                    {
                        RequireHttps =
                            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development"
                    }
                },
                ct);

        if (disco.IsError)
            throw new InvalidOperationException(disco.Error);

        TokenResponse token =
            await _http.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _auth.ClientId,
                    ClientSecret = _auth.ClientSecret,
                    Scope = _auth.Scope
                },
                ct);

        if (token.IsError || token.AccessToken is null)
            throw new InvalidOperationException(token.Error ?? "No access token received.");

        _cachedAccessToken = token.AccessToken;
        _accessTokenExpiresAt =
            DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);

        return _cachedAccessToken;
    }

    private async Task AuthorizeAsync(CancellationToken ct)
    {
        string token = await GetValidAccessTokenAsync(ct);
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    // Batch Notify Matches Finished
    public async Task NotifyMatchesFinishedBatchAsync(
        IReadOnlyList<MatchFinishedRequest> matches,
        CancellationToken ct)
    {
        if (matches.Count == 0)
        {
            return;
        }

        await AuthorizeAsync(ct);

        HttpResponseMessage response =
            await _http.PostAsJsonAsync(
                "internal/matches/finished/batch",
                new MatchFinishedBatchRequest(matches),
                ct);

        response.EnsureSuccessStatusCode();
    }
}
