using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Images;

namespace HotelCore.Infrastructure.Persistence.Configurations;

public class MyImageConfiguration : IEntityTypeConfiguration<MyImage>
{
    public void Configure(EntityTypeBuilder<MyImage> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.StorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(i => i.Type)
            .IsRequired();

        builder.HasIndex(i => new { i.ImageGroupId, i.Type });
    }
}

public class MyImageGroupConfiguration : IEntityTypeConfiguration<MyImageGroup>
{
    public void Configure(EntityTypeBuilder<MyImageGroup> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title)
            .HasMaxLength(500);

        builder.Property(g => g.Type)
            .IsRequired();

        builder.HasMany(g => g.Images)
            .WithOne(i => i.ImageGroup)
            .HasForeignKey(i => i.ImageGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(g => new { g.OrderId, g.Position });
        builder.HasIndex(g => new { g.WorkerPortfolioItemId, g.Position });

        builder.HasQueryFilter(g => !g.IsDeleted);
    }
}
