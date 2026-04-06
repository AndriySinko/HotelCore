using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Seekers;

namespace HotelCore.Infrastructure.Persistence.Configurations.Seekers;

public class SeekerProfileConfiguration : IEntityTypeConfiguration<SeekerProfile>
{
    public void Configure(EntityTypeBuilder<SeekerProfile> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Rating)
            .HasPrecision(3, 2);

        builder.HasOne(sp => sp.User)
            .WithOne(u => u.SeekerProfile)
            .HasForeignKey<SeekerProfile>(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.DefaultLocation)
            .WithMany()
            .HasForeignKey(sp => sp.DefaultLocationId);

        builder.HasQueryFilter(sp => !sp.IsDeleted);
    }
}
