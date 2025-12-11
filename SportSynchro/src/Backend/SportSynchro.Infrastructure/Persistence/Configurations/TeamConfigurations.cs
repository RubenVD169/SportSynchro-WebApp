using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.Configurations;

public sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.LeagueId, t.ExternalId })
               .IsUnique();

        builder.Property(t => t.ExternalId)
               .IsRequired();

        builder.Property(t => t.LeagueId)
               .IsRequired();

        builder.Property(t => t.Country)
               .HasMaxLength(50)
               .IsRequired();

        builder.ComplexProperty(t => t.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.HasOne<League>()
               .WithMany()
               .HasForeignKey(t => t.LeagueId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
