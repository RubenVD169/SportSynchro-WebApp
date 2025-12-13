using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Domain.Entities;

public sealed class UserFavorite
{
    private UserFavorite() { } // EF Core only

    public UserFavorite(string aspUserId, int teamId)
    {
        if (string.IsNullOrWhiteSpace(aspUserId))
            throw new UserFavoriteException("AspUserId must be provided.");

        if (teamId <= 0)
            throw new UserFavoriteException("TeamId must be positive.");

        AspUserId = aspUserId;
        TeamId = teamId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string AspUserId { get; private set; }
    public int TeamId { get; private set; }
    public DateTime CreatedAtUtc { get; private init; }
}
