using System.Text.Json;
using Microsoft.Azure.Cosmos;

namespace SportSynchro.LiveScore.Api;

public sealed class SystemTextJsonCosmosSerializer : CosmosSerializer
{
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonCosmosSerializer(JsonSerializerOptions options)
    {
        _options = options;
    }

    public override T FromStream<T>(Stream stream)
    {
        if (stream is { CanSeek: true, Length: 0 })
            return default!;

        using (stream)
        {
            return JsonSerializer.Deserialize<T>(stream, _options)!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream stream = new();
        JsonSerializer.Serialize(stream, input, _options);
        stream.Position = 0;
        return stream;
    }
}