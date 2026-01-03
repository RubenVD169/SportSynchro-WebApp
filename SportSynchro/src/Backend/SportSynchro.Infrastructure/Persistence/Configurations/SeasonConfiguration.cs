using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.Configurations;

public sealed class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
       public void Configure(EntityTypeBuilder<Season> builder)
       {
              builder.ToTable("Seasons");

              builder.HasKey(s => s.Id);

              builder.Property(s => s.LeagueId)
                     .IsRequired();

              builder.Property(s => s.IsCurrent)
                     .IsRequired();

              builder.Property(s => s.TeamsImported)
                     .IsRequired();

              builder.Property(s => s.MatchesImported)
                     .IsRequired();

              builder.Property(s => s.MatchesImportedUntilUtc)
                     .HasColumnType("datetime2")
                     .IsRequired(false);

              builder.ComplexProperty(s => s.Key, key =>
              {
                     key.Property(k => k.Value)
                  .HasColumnName("SeasonKey")
                  .HasMaxLength(50)
                  .IsRequired();
              });

              builder.HasIndex(s => s.LeagueId);

              builder.HasOne<League>()
                     .WithMany()
                     .HasForeignKey(s => s.LeagueId)
                     .OnDelete(DeleteBehavior.Restrict);
       }
}
