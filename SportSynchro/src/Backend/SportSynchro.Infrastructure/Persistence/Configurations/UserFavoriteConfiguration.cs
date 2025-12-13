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

              builder.Property(f => f.AspUserId)
                     .IsRequired()
                     .HasMaxLength(450); // Identity default 


              builder.Property(f => f.TeamId)
               .IsRequired();

              builder.Property(f => f.CreatedAtUtc)
                     .IsRequired()
                     .HasDefaultValueSql("SYSUTCDATETIME()");

              // Prevent duplicate favorites
              builder.HasIndex(f => new { f.AspUserId, f.TeamId })
                     .IsUnique();

              builder.HasOne<Team>()
                     .WithMany()
                     .HasForeignKey(f => f.TeamId)
                     .OnDelete(DeleteBehavior.Restrict);
       }
}
