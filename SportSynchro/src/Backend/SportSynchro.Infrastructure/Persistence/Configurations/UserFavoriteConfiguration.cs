using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.Configurations;

public sealed class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
{
    public void Configure(EntityTypeBuilder<UserFavorite> builder)
    {
        builder.ToTable("UserFavorites");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.UserId)
               .IsRequired();

        builder.Property(f => f.TeamId)
               .IsRequired();

        builder.Property(f => f.CreatedAtUtc)
               .IsRequired();

        // Prevent duplicate favorites
        builder.HasIndex(f => new { f.UserId, f.TeamId })
               .IsUnique();

        builder.HasOne<Team>()
               .WithMany()
               .HasForeignKey(f => f.TeamId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
