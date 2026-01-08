namespace SportSynchro.Infrastructure.Blob.Options;

public sealed class ScheduleBlobOptions
{
    public const string SectionName = "ScheduleBlob";
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}