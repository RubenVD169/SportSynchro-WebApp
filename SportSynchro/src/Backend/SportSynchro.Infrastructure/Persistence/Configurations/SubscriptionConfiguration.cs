using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.AspUserId)
               .IsRequired();

        builder.Property(s => s.CreatedAtUtc)
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");


        builder.ComplexProperty(s => s.Type, type =>
        {
            type.Property(t => t.Value)
                .HasColumnName("SubscriptionType")
                .HasMaxLength(50)
                .IsRequired();
        });

        // Ensure 1 active lifetime subscription per user
        builder.HasIndex(s => s.AspUserId)
               .IsUnique();
    }
}
