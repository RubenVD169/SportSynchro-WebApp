using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SportSynchro.LiveScore.Api.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using SportSynchro.LiveScore.Api.Options;
using System.Net;

namespace SportSynchro.LiveScore.Api.Infrastructure;

public sealed class CosmosLiveMatchRepository : ILiveMatchRepository
{
    private readonly CosmosOptions _options;
    private Container? _container;

    public CosmosLiveMatchRepository(IOptions<CosmosOptions> options)
    {
        _options = options.Value;
    }

    private Container GetCosmosContainer()
    {
        if (_container is not null)
            return _container;

        JsonSerializerOptions serializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        CosmosClientOptions clientOptions = new()
        {
            Serializer = new SystemTextJsonCosmosSerializer(serializerOptions)
        };

        CosmosClient client = new(_options.Connectionstring, clientOptions);

        Database? database = client.GetDatabase(_options.DatabaseName);
        _container = database.GetContainer(_options.ContainerName);

        return _container;
    }

    public async Task UpsertAsync(LiveMatchDocument match, CancellationToken ct = default)
    {
        Container container = GetCosmosContainer();

        await container.UpsertItemAsync(
            match,
            new PartitionKey(match.IdLeague),
            cancellationToken: ct);
    }

    public async Task<IReadOnlyList<LiveMatchDocument>>
        GetByLeagueAsync(
            string leagueId,
            CancellationToken ct = default)
    {
        Container container = GetCosmosContainer();

        QueryDefinition? query = new QueryDefinition(
            "SELECT * FROM c WHERE c.idLeague = @leagueId")
            .WithParameter("@leagueId", leagueId);

        FeedIterator<LiveMatchDocument> iterator = container.GetItemQueryIterator<LiveMatchDocument>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(leagueId)
            });

        List<LiveMatchDocument> results = [];

        while (iterator.HasMoreResults)
        {
            FeedResponse<LiveMatchDocument>? response = await iterator.ReadNextAsync(ct);
            results.AddRange(response);
        }

        return results;
    }

    public async Task<LiveMatchDocument?>
        GetByEventAsync(
            string eventId,
            string leagueId,
            CancellationToken ct = default)
    {
        Container container = GetCosmosContainer();
        
        try
        {
            ItemResponse<LiveMatchDocument>? response = await container.ReadItemAsync<LiveMatchDocument>(
                id: $"event-{eventId}",
                partitionKey: new PartitionKey(leagueId),
                cancellationToken: ct);

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<LiveMatchDocument?> TryGetAsync(
    string id,
    string partitionKey,
    CancellationToken ct)
    {
        Container container = GetCosmosContainer();
        try
        {
            ItemResponse<LiveMatchDocument> response =
                await container.ReadItemAsync<LiveMatchDocument>(
                    id,
                    new PartitionKey(partitionKey),
                    cancellationToken: ct);

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
