namespace SportSynchro.LiveScore.Api.Options;

public sealed class CosmosOptions
{
    public const string SectionName = "Cosmos";

    public required string Connectionstring { get; init; }
    public required string DatabaseName { get; init; }
    public required string ContainerName { get; init; }
}
