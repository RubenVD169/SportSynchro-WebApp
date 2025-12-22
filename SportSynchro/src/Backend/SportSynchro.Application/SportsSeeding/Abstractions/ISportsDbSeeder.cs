namespace SportSynchro.Application.SportsSeeding.Abstractions;

public interface ISportsDbSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
