using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Identity;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(e => e.StudentNumber).IsRequired().HasMaxLength(20);
            builder.HasIndex(e => e.StudentNumber).IsUnique();
        }
    }
}