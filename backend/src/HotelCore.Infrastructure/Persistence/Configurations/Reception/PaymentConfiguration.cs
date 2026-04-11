// This file contains code for PaymentConfiguration.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Reception;

namespace HotelCore.Infrastructure.Persistence.Configurations.Reception;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Property(p => p.Amount)
            .HasPrecision(10, 2);

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(100);
    }
}
