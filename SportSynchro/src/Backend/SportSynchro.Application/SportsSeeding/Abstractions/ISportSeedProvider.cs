using SportSynchro.Application.SportsSeeding.Models;

namespace SportSynchro.Application.SportsSeeding.Abstractions;

public interface ISportsSeedProvider
{
    Task<IReadOnlyList<SportSeedModel>> LoadSeedDataAsync(
        CancellationToken cancellationToken = default);
}
