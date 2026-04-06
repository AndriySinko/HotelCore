using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Reviews;

namespace HotelCore.Infrastructure.Persistence.Configurations.Reviews;

public class ReviewMediaConfiguration : IEntityTypeConfiguration<ReviewMedia>
{
    public void Configure(EntityTypeBuilder<ReviewMedia> builder)
    {
        builder.HasKey(rm => rm.Id);

        builder.HasQueryFilter(rm => !rm.IsDeleted);
    }
}
