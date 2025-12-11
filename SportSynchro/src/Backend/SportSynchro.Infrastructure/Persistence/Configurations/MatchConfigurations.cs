using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.Configurations;

public sealed class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("Matches");

        builder.HasKey(m => m.Id);

        builder.HasIndex(m => new { m.LeagueId, m.ExternalId })
               .IsUnique();

        builder.Property(m => m.ExternalId)
               .IsRequired();

        builder.Property(m => m.LeagueId)
               .IsRequired();

        builder.Property(m => m.HomeTeamId)
               .IsRequired();

        builder.Property(m => m.AwayTeamId)
               .IsRequired();

        builder.Property(m => m.StartTimeUtc)
               .IsRequired();

        // Map status
        builder.ComplexProperty(m => m.Status, status =>
        {
            status.Property(s => s.Value)
                  .HasColumnName("Status")
                  .HasMaxLength(20)
                  .IsRequired();
        });

        builder.HasOne<League>()
               .WithMany()
               .HasForeignKey(m => m.LeagueId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Team>()
               .WithMany()
               .HasForeignKey(m => m.HomeTeamId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Team>()
               .WithMany()
               .HasForeignKey(m => m.AwayTeamId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
