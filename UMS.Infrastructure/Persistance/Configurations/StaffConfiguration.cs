using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Identity;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(20);
            builder.Property(e => e.JobTitle).IsRequired().HasMaxLength(100);

            builder.HasIndex(e => e.EmployeeNumber).IsUnique();
        }
    }
}