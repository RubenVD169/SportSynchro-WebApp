using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using SportSynchro.Application.Interfaces.Blob;
using SportSynchro.Infrastructure.Blob.Options;

namespace SportSynchro.Infrastructure.Blob;

public sealed class SchedulePdfRepository : ISchedulePdfRepository
{
    private readonly BlobContainerClient _containerClient;

    public SchedulePdfRepository(IOptions<ScheduleBlobOptions> options)
    {
        var blobOptions = options.Value;

        var serviceClient =
            new BlobServiceClient(blobOptions.ConnectionString);

        _containerClient =
            serviceClient.GetBlobContainerClient(
                blobOptions.ContainerName);
    }

    private BlobClient GetBlobClient(int leagueId, int seasonId)
    {
        string path = $"league-{leagueId}_season-{seasonId}_schedule.pdf";

        return _containerClient.GetBlobClient(path);
    }

    public async Task<byte[]> ReadAsync(
        int leagueId,
        int seasonId)
    {
        var blob = GetBlobClient(leagueId, seasonId);

        var download =
            await blob.DownloadContentAsync();

        return download.Value.Content.ToArray();
    }

    public async Task WriteAsync(
        byte[] content,
        int leagueId,
        int seasonId)
    {
        var blob = GetBlobClient(leagueId, seasonId);

        await blob.UploadAsync(
            new BinaryData(content),
            overwrite: true);
    }
}
