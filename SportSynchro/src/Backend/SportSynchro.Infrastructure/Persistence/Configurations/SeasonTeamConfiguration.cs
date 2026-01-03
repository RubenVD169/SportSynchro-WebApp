using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.Configurations;

public sealed class SeasonTeamConfiguration : IEntityTypeConfiguration<SeasonTeam>
{
    public void Configure(EntityTypeBuilder<SeasonTeam> builder)
    {
        builder.ToTable("SeasonTeams");

        builder.HasKey(st => new { st.SeasonId, st.TeamId });

        builder.Property(st => st.SeasonId)
               .IsRequired();

        builder.Property(st => st.TeamId)
               .IsRequired();

        builder.HasOne<Season>()
               .WithMany()
               .HasForeignKey(st => st.SeasonId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Team>()
               .WithMany()
               .HasForeignKey(st => st.TeamId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
