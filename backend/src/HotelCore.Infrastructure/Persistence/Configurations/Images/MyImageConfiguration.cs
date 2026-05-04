using HotelCore.Domain.Entities.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCore.Infrastructure.Persistence.Configurations.Images;

public class MyImageConfiguration : IEntityTypeConfiguration<MyImage>
{
    public void Configure(EntityTypeBuilder<MyImage> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.StorageKey)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(2048);
    }
}
