using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.Configurations;

public sealed class SportConfiguration : IEntityTypeConfiguration<Sport>
{
    public void Configure(EntityTypeBuilder<Sport> builder)
    {
        builder.ToTable("Sports");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ExternalId)
               .IsRequired();

        builder.Property(s => s.IsVisible)
               .IsRequired();

        // Unique external import constraint
        builder.HasIndex(s => s.ExternalId)
               .IsUnique();

        // Map ValueObject
        builder.ComplexProperty(s => s.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();
        });
    }
}
