using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Enums;

namespace HotelCore.Infrastructure.Persistence.Configurations.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.Description)
            .HasMaxLength(5000);

        builder.Property(o => o.Price)
            .HasPrecision(18, 2);

        builder.Property(o => o.PaymentType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.IsHidden)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(OrderType.Regular);

        builder.HasOne(o => o.CreatedByUser)
            .WithMany()
            .HasForeignKey(o => o.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Client)
            .WithOne(c => c.Order)
            .HasForeignKey<OrderClient>(c => c.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.ParentOrder)
            .WithMany(o => o.SubOrders)
            .HasForeignKey(o => o.ParentOrderId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(o => o.Images)
            .WithOne(g => g.Order)
            .HasForeignKey(g => g.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
