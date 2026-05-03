using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.StaffManagement;

namespace HotelCore.Infrastructure.Persistence.Configurations.StaffManagement;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasOne(s => s.AssignedEmployee)
            .WithMany()
            .HasForeignKey(s => s.StaffMemberId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(s => s.RequiredRole).HasMaxLength(100);
    }
}
