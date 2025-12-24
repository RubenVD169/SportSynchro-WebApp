using SportSynchro.Application.Models.Sports;

namespace SportSynchro.Application.Interfaces.Services;

public interface ISportService
{
  Task<IReadOnlyList<SportAdminModel>> GetAllForAdminAsync(
    CancellationToken cancellationToken = default);
  Task SetVisibilityAsync(
    int sportId,
    bool isVisible, 
    CancellationToken cancellationToken = default);
}