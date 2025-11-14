using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Academic;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration : BaseEntityConfiguration<Course>
    {
        public override void Configure(EntityTypeBuilder<Course> builder)
        {
            base.Configure(builder);

            builder.ToTable("Courses");

            builder.Property(e => e.Code).IsRequired().HasMaxLength(20);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).HasMaxLength(1000);

            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}