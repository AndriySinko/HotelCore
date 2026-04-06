using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Workers;

namespace HotelCore.Infrastructure.Persistence.Configurations.Workers;

public class WorkerServiceListingConfiguration : IEntityTypeConfiguration<WorkerServiceListing>
{
    public void Configure(EntityTypeBuilder<WorkerServiceListing> builder)
    {
        builder.HasKey(wsl => wsl.Id);

        builder.Property(wsl => wsl.StartingPrice)
            .HasPrecision(18, 2);

        builder.HasOne(wsl => wsl.WorkerProfile)
            .WithMany()
            .HasForeignKey(wsl => wsl.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wsl => wsl.Category)
            .WithMany()
            .HasForeignKey(wsl => wsl.CategoryId);

        builder.HasOne(wsl => wsl.Location)
            .WithMany()
            .HasForeignKey(wsl => wsl.LocationId);

        builder.HasQueryFilter(wsl => !wsl.IsDeleted);
    }
}
