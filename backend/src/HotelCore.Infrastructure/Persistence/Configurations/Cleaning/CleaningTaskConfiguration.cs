// This file contains code for CleaningTaskConfiguration.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Cleaning;

namespace HotelCore.Infrastructure.Persistence.Configurations.Cleaning;

public class CleaningTaskConfiguration : IEntityTypeConfiguration<CleaningTask>
{
    public void Configure(EntityTypeBuilder<CleaningTask> builder)
    {
        builder.HasQueryFilter(task => !task.IsDeleted);

        builder.HasOne(task => task.Room)
            .WithMany(room => room.CleaningTasks)
            .HasForeignKey(task => task.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(task => task.AssignedStaff)
            .WithMany()
            .HasForeignKey(task => task.AssignedStaffId)
            .OnDelete(DeleteBehavior.SetNull);

        
        
        builder.Property(task => task.CancellationReason)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}
