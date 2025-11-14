using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Identity;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
    {
        public void Configure(EntityTypeBuilder<Faculty> builder)
        {
            builder.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(20);
            builder.Property(e => e.OfficeLocation).HasMaxLength(50);
            builder.Property(e => e.OfficeHours).HasMaxLength(200);

            builder.HasIndex(e => e.EmployeeNumber).IsUnique();
        }
    }
}