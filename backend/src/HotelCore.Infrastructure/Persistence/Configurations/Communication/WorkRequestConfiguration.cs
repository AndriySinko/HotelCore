using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Communication;

namespace HotelCore.Infrastructure.Persistence.Configurations.Communication;

public class WorkRequestConfiguration : IEntityTypeConfiguration<WorkRequest>
{
    public void Configure(EntityTypeBuilder<WorkRequest> builder)
    {
        builder.HasKey(wr => wr.Id);

        builder.Property(wr => wr.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(wr => wr.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(wr => wr.Budget)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(wr => wr.SeekerProfile)
            .WithMany()
            .HasForeignKey(wr => wr.SeekerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wr => wr.Category)
            .WithMany()
            .HasForeignKey(wr => wr.CategoryId);

        builder.HasOne(wr => wr.Location)
            .WithMany()
            .HasForeignKey(wr => wr.LocationId);

        builder.HasMany(wr => wr.Offers)
            .WithOne(o => o.WorkRequest)
            .HasForeignKey(o => o.WorkRequestId);
            
        builder.HasQueryFilter(wr => !wr.IsDeleted);
    }
}
