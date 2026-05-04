using HotelCore.Domain.Entities.Users.Restaurant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCore.Infrastructure.Persistence.Configurations.Restaurant;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);  // standard for money — avoids floating-point rounding

        builder.Property(x => x.IsAvailable)
            .HasDefaultValue(true);

        // Restrict: don't cascade-delete an image when the product is soft-deleted.
        // Image cleanup (R2 storage + DB row) goes through the storage service separately.
        builder.HasOne(x => x.Image)
            .WithOne(x => x.Product)
            .HasForeignKey<Product>(x => x.ImageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ImageId)
            .IsUnique();
    }
}
