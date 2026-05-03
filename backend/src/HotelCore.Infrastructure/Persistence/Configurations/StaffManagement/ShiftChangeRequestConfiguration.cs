using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.StaffManagement;

namespace HotelCore.Infrastructure.Persistence.Configurations.StaffManagement;

public class ShiftChangeRequestConfiguration : IEntityTypeConfiguration<ShiftChangeRequest>
{
    public void Configure(EntityTypeBuilder<ShiftChangeRequest> builder)
    {
        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasOne(r => r.Shift)
            .WithMany()
            .HasForeignKey(r => r.ShiftId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.RequestedBy)
            .WithMany()
            .HasForeignKey(r => r.RequestedByStaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReviewedBy)
            .WithMany()
            .HasForeignKey(r => r.ReviewedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(r => r.Reason).HasMaxLength(500);
        builder.Property(r => r.ReviewerComment).HasMaxLength(500);
    }
}
