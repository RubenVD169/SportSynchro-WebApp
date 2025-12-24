namespace SportSynchro.Infrastructure.Persistence.Seeding;

public interface ISportsDbSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
