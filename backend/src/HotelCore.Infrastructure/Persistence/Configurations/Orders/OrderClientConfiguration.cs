using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Orders;

namespace HotelCore.Infrastructure.Persistence.Configurations.Orders;

public class OrderClientConfiguration : IEntityTypeConfiguration<OrderClient>
{
    public void Configure(EntityTypeBuilder<OrderClient> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .HasMaxLength(255);

        builder.Property(c => c.GuestAccessToken)
            .HasMaxLength(100);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.Email);
        builder.HasIndex(c => c.PhoneNumber);
        builder.HasIndex(c => c.GuestAccessToken);
    }
}
