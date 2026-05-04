using HotelCore.Domain.Entities.Users.Restaurant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCore.Infrastructure.Persistence.Configurations.Restaurant;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Unique index enforces the 1:1 at DB level — HasOne/WithOne alone doesn't prevent a second row.
        builder.HasIndex(x => x.OrderId)
            .IsUnique();
    }
}
