using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Users;

namespace HotelCore.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Identity handles Email, PhoneNumber, PasswordHash etc.
        
        builder.Property(u => u.FirstName)
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .HasMaxLength(100);

        builder.HasOne(u => u.Avatar)
            .WithMany()
            .HasForeignKey(u => u.AvatarId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.WorkerProfile)
            .WithOne(wp => wp.User)
            .HasForeignKey<Domain.Entities.Workers.WorkerProfile>(wp => wp.UserId);

        builder.HasOne(u => u.Company)
            .WithOne(c => c.Owner)
            .HasForeignKey<Domain.Entities.Companies.Company>(c => c.OwnerId);
            
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
