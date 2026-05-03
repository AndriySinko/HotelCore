using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.StaffManagement;

namespace HotelCore.Infrastructure.Persistence.Configurations.StaffManagement;

public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasMany(s => s.Shifts)
            .WithOne(sh => sh.WorkSchedule)
            .HasForeignKey(sh => sh.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
