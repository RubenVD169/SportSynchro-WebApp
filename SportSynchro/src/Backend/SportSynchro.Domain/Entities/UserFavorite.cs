using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Domain.Entities;

public sealed class UserFavorite
{
    private UserFavorite() { } // EF Core only

    public UserFavorite(int userId, int teamId)
    {
        if (userId <= 0)
            throw new UserFavoriteException("User ID must be positive.");

        if (teamId <= 0)
            throw new UserFavoriteException("Team ID must be positive.");

        UserId = userId;
        TeamId = teamId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int TeamId { get; private set; }
    public DateTime CreatedAtUtc { get; private init; }
}
