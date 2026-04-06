using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Payments;

namespace HotelCore.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        builder.Property(p => p.Currency).HasMaxLength(3).HasDefaultValue("EUR");
        builder.Property(p => p.ExternalTransactionId).HasMaxLength(100);

        builder.HasIndex(p => p.ExternalTransactionId);

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
