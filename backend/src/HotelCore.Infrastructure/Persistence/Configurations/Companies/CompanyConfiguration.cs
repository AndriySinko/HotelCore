using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelCore.Domain.Entities.Companies;

namespace HotelCore.Infrastructure.Persistence.Configurations.Companies;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.RegistrationNumber)
            .HasMaxLength(20);

        builder.Property(c => c.TaxNumber)
            .HasMaxLength(20);

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Company)
            .HasForeignKey(m => m.CompanyId);
            
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
