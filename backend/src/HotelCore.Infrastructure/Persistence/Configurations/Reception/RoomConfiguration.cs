using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Reception;

namespace HotelCore.Infrastructure.Persistence.Configurations.Reception;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.Property(r => r.RoomNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(r => r.PricePerNight)
            .HasPrecision(10, 2);

        builder.HasIndex(r => r.RoomNumber).IsUnique();
    }
}
