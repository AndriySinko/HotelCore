using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Workers;

namespace HotelCore.Infrastructure.Persistence.Configurations.Workers;

public class WorkerMediaConfiguration : IEntityTypeConfiguration<WorkerMedia>
{
    public void Configure(EntityTypeBuilder<WorkerMedia> builder)
    {
        builder.HasKey(wm => wm.Id);
        builder.HasQueryFilter(wm => !wm.IsDeleted);
    }
}

public class WorkerCategoryConfiguration : IEntityTypeConfiguration<WorkerCategory>
{
    public void Configure(EntityTypeBuilder<WorkerCategory> builder)
    {
        builder.HasKey(wc => wc.Id);
        builder.HasQueryFilter(wc => !wc.IsDeleted);
    }
}

public class WorkerPortfolioItemConfiguration : IEntityTypeConfiguration<WorkerPortfolioItem>
{
    public void Configure(EntityTypeBuilder<WorkerPortfolioItem> builder)
    {
        builder.HasKey(wp => wp.Id);
        
        builder.Property(wp =>wp.IsHidden)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.HasMany(wp => wp.Images)
            .WithOne(g => g.WorkerPortfolioItem)
            .HasForeignKey(g => g.WorkerPortfolioItemId);

        builder.HasQueryFilter(wp => !wp.IsDeleted);
    }
}
