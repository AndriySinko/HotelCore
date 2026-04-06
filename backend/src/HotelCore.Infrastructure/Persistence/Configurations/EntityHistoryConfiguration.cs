using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Common;

namespace HotelCore.Infrastructure.Persistence.Configurations;

public class EntityHistoryConfiguration : IEntityTypeConfiguration<EntityHistory>
{
    public void Configure(EntityTypeBuilder<EntityHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.EntityType)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(h => h.PropertyName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(h => h.OldValue)
            .HasMaxLength(4000);

        builder.Property(h => h.NewValue)
            .HasMaxLength(4000);

        builder.Property(h => h.ChangeDescription)
            .HasMaxLength(1000);

        builder.Property(h => h.ChangedAt)
            .IsRequired();

        builder.HasIndex(h => new { h.EntityType, h.EntityId, h.ChangedAt })
            .HasDatabaseName("IX_EntityHistory_Entity_ChangedAt");

        builder.HasIndex(h => h.ChangedByUserId);

        builder.HasIndex(h => h.ChangedAt);
    }
}
