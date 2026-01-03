using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.Configurations;

public sealed class LeagueConfiguration : IEntityTypeConfiguration<League>
{
       public void Configure(EntityTypeBuilder<League> builder)
       {
              builder.ToTable("Leagues");

              builder.HasKey(l => l.Id);

              builder.HasIndex(l => new { l.SportId, l.ExternalId })
                     .IsUnique();

              builder.Property(l => l.ExternalId)
                     .IsRequired();

              builder.Property(l => l.SportId)
                     .IsRequired();

              builder.Property(l => l.IsVisible)
                     .IsRequired();

              builder.ComplexProperty(l => l.Name, name =>
              {
                     name.Property(n => n.Value)
                   .HasColumnName("Name")
                   .HasMaxLength(200)
                   .IsRequired();
              });

              builder.HasOne<Sport>()
                     .WithMany()
                     .HasForeignKey(l => l.SportId)
                     .OnDelete(DeleteBehavior.Restrict);
       }
}
